using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Reviews.Convenience;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.DataAccess.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Enums;
using iRLeagueDatabase.DataAccess.Provider.Generic;

namespace iRLeagueDatabase.DataAccess.Provider
{
    /// <summary>
    /// Data access class for retrieving review convenience data from League database
    /// </summary>
    public class ReviewDataProvider : DataProviderBase, IReviewDataProvider
    {
        /// <summary>
        /// Create new instance provided with an existing database context
        /// </summary>
        /// <param name="context">Provider context containing model store and user information</param>
        public ReviewDataProvider(IProviderContext<LeagueDbContext> context) : base(context)
        {
        }

        /// <summary>
        /// Get all reviews belonging to a season, specified by its seasonId
        /// </summary>
        /// <param name="seasonId">Id of the seasson</param>
        /// <returns>DTO containing all reviews and summary data</returns>
        public SeasonReviewsDTO GetReviewsFromSeason(long seasonId)
        {
            // load season from db
            SeasonEntity season;
            if (seasonId == 0)
            {
                season = DbContext.Set<SeasonEntity>().OrderByDescending(x => x.SeasonStart).FirstOrDefault();
            }
            else
            {
                season = DbContext.Set<SeasonEntity>().Find(seasonId);
            }

            if (season == null)
            {
                return new SeasonReviewsDTO() { SeasonId = seasonId };
            }

            // get season sessions
            var sessions = season.Schedules.SelectMany(x => x.Sessions);
            // preload reviews and vote cats
            var reviewDtos = GetReviews(sessions.Select(x => x.SessionId).ToArray()).GroupBy(x => x.SessionId).ToDictionary(x => x.Key, x => x.ToArray());
            DbContext.Set<VoteCategoryEntity>().Load();

            var sessionReviews = sessions.Select(x => GetReviewsFromSession(x.SessionId, x, reviewDtos.TryGetValue(x.SessionId, out IncidentReviewDataDTO[] reviews) ? reviews : new IncidentReviewDataDTO[0])).ToArray();

            // construct DTO
            var seasonReviews = new SeasonReviewsDTO()
            {
                ReviewsCount = sessionReviews.Sum(x => x.Total),
                SessionReviews = sessionReviews,
                SchedulesCount = season.Schedules.Count(),
                SeasonId = seasonId,
                SessionCount = sessions.Count()
            };

            return seasonReviews;
        }

        /// <summary>
        /// Get all reviews belonging to a session, specified by its sessionId
        /// </summary>
        /// <param name="sessionId">Id of the session</param>
        /// <returns>DTO containing all reviews and summary data</returns>
        public SessionReviewsDTO GetReviewsFromSession(long sessionId, SessionBaseEntity preLoadedSession = null, IncidentReviewDataDTO[] preLoadedReviews = null)
        {
            // Load session from db
            SessionBaseEntity session;
            // if session id == 0 load latest session, else load specified session
            if (sessionId == 0 && preLoadedSession == null)
            {
                session = DbContext.Set<SessionBaseEntity>()
                    .Where(x => x.SessionResult != null)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
            }
            else if (preLoadedSession == null)
            {
                session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
            }
            else
            {
                session = preLoadedSession;
            }

            if (session == null)
            {
                return new SessionReviewsDTO();
            }

            var mapper = new DTOMapper(DbContext);

            // get session race number
            int raceNr = 0;
            if (session.SessionType == iRLeagueManager.Enums.SessionType.Race)
            {
                var season = session.Schedule.Season;
                var seasonSessions = season.Schedules.SelectMany(x => x.Sessions).Where(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race).OrderBy(x => x.Date);
                raceNr = (seasonSessions.Select((x, i) => new { number = i + 1, item = x }).FirstOrDefault(x => x.item.SessionId == sessionId)?.number).GetValueOrDefault();
            }

            IncidentReviewDataDTO[] reviews = preLoadedReviews;
            if (preLoadedReviews == null)
            // get all reviews ids for this session and retrieve reviews data from ModelDataProvider
            {
                //var reviewIds = session.Reviews.Select(x => x.ReviewId);
                var modelDataProvider = new ModelDataProvider(ProviderContext);
                reviews = GetReviews(session.SessionId);
            }

            // get custom vote categories information from database
            var voteCatIds = reviews.Where(x => x.AcceptedReviewVotes?.Count() > 0).SelectMany(x => x.AcceptedReviewVotes.Select(y => y.VoteCategoryId)).Distinct();
            var voteCats = DbContext.Set<VoteCategoryEntity>().Local.Where(x => voteCatIds.Contains(x.CatId)).ToList().Select(x => mapper.MapToVoteCategoryDTO(x));
            if (voteCats.Count() < voteCatIds.Count())
            {
                voteCats = DbContext.Set<VoteCategoryEntity>().Where(x => voteCatIds.Contains(x.CatId)).ToList().Select(x => mapper.MapToVoteCategoryDTO(x));
            }

            /* construct DTOs */
            // get all vote results that resulted in a penalty
            var penalties = reviews
                .Where(x => x.AcceptedReviewVotes?.Count() > 0)
                .SelectMany(x => x.AcceptedReviewVotes)
                .Where(x => x.CatPenalty > 0 && x.MemberAtFaultId != null);
            // summarize penalites for each driver
            var driverPenalties = penalties
                .GroupBy(x => x.MemberAtFaultId)
                .Select(x => new MemberPenaltySummaryDTO()
                {
                    MemberId = x.Key.GetValueOrDefault(),
                    Name = DbContext.Set<LeagueMemberEntity>().Find(x.Key.GetValueOrDefault()).Fullname,
                    Count = x.Count(),
                    Points = x.Sum(y => y.CatPenalty),
                    Penalties = x.ToArray()
                });
            // summarized penalties for all reviews
            var penaltySummary = new ReviewsPenaltySummaryDTO()
            {
                Count = driverPenalties.Sum(x => x.Count),
                Points = driverPenalties.Sum(x => x.Points),
                DrvPenalties = driverPenalties.ToArray()
            };
            // create review convencience DTO
            var reviewData = new SessionReviewsDTO()
            {
                Reviews = reviews,
                Total = reviews.Count(),
                Open = reviews.Count(x => x.AcceptedReviewVotes == null || x.AcceptedReviewVotes.Count() == 0),
                Voted = reviews.Count(x => x is IncidentReviewDataDTO review ? review.Comments.Any(y => y.CommentReviewVotes.Count() > 0) : false),
                Closed = reviews.Count(x => x.AcceptedReviewVotes?.Count() > 0),
                Penalties = penaltySummary,
                Results = reviews
                    .Where(x => x.AcceptedReviewVotes?.Count() > 0)
                    .SelectMany(x => x.AcceptedReviewVotes)
                    .Where(x => x.VoteCategoryId != null)
                    .GroupBy(x => x.VoteCategoryId)
                    .Select(x => new CountValue<VoteCategoryDTO>() { Count = x.Count(), Value = voteCats.SingleOrDefault(y => y.CatId == x.Key.Value) })
                    .ToArray(),
                SessionId = sessionId,
                RaceNr = raceNr
            };
            /* END construct DTOs */

            return reviewData;
        }

        public IncidentReviewDataDTO[] GetReviews(params long[] sessionIds)
        {
            var mapper = new DTOMapper(DbContext);

            DbContext.Configuration.LazyLoadingEnabled = false;
            var reviewEnties = DbContext.Set<IncidentReviewEntity>().Where(x => sessionIds.Contains(x.SessionId))
                .Include(x => x.Session)
                .Include(x => x.InvolvedMembers).ToArray();
            var reviewIds = reviewEnties.Select(x => x.ReviewId);

            DbContext.Set<ReviewCommentEntity>().Where(x => reviewIds.Contains(x.ReviewId))
                .Include(x => x.CommentReviewVotes.Select(y => y.MemberAtFault))
                .Include(x => x.Replies).Load();
            DbContext.Set<AcceptedReviewVoteEntity>().Where(x => reviewIds.Contains(x.ReviewId))
                .Include(x => x.MemberAtFault)
                .Include(x => x.CustomVoteCat)
                .Load();

            DbContext.ChangeTracker.DetectChanges();

            IncidentReviewDataDTO[] reviewDtos;
            if (LeagueRoles.HasFlag(LeagueRoleEnum.Steward))
            {
                reviewDtos = reviewEnties
                    .Where(x => CheckLeague(DbContext.CurrentLeagueId, x))
                    .Select(x => mapper.MapToReviewDataDTO(x))
                    .ToArray();
            }
            else
            {
                reviewDtos = reviewEnties
                    .Where(x => CheckLeague(DbContext.CurrentLeagueId, x))
                    .Select(x => mapper.MapToPublicReviewDataDTO(x))
                    .OfType<IncidentReviewDataDTO>()
                    .ToArray();
            }
            DbContext.Configuration.LazyLoadingEnabled = true;

            return reviewDtos;
        }
    }
}