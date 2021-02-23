﻿using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Reviews.Convenience;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using iRLeagueDatabase.Entities;

namespace iRLeagueRESTService.Data
{
    /// <summary>
    /// Data access class for retrieving review convenience data from League database
    /// </summary>
    public class ReviewDataProvider : IReviewDataProvider
    {
        /// <summary>
        /// Data context of the database
        /// </summary>
        private LeagueDbContext DbContext { get; }

        /// <summary>
        /// Create new instance provided with an existing database context
        /// </summary>
        /// <param name="dbContext">Data context of the database</param>
        public ReviewDataProvider(LeagueDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// Get all reviews belonging to a season, specified by its seasonId
        /// </summary>
        /// <param name="seasonId">Id of the seasson</param>
        /// <returns>DTO containing all reviews and summary data</returns>
        public SeasonReviewsDTO GetReviewsFromSeason(long seasonId)
        {
            // load season from db
            var season = DbContext.Set<SeasonEntity>().Find(seasonId);

            if (season == null)
            {
                return new SeasonReviewsDTO() { SeasonId = seasonId };
            }

            // get season sessions
            var sessions = season.Schedules.SelectMany(x => x.Sessions);
            var sessionReviews = sessions.Select(x => GetReviewsFromSession(x.SessionId)).ToArray();

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
        public SessionReviewsDTO GetReviewsFromSession(long sessionId)
        {
            // Load session from db
            SessionBaseEntity session;
            // if session id == 0 load latest session, else load specified session
            if (sessionId == 0)
            {
                session = DbContext.Set<SessionBaseEntity>()
                    .Where(x => x.SessionResult != null)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
            }
            else
            {
                session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
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

            // get all reviews ids for this session and retrieve reviews data from ModelDataProvider
            var reviewIds = session.Reviews.Select(x => x.ReviewId);
            var modelDataProvider = new ModelDataProvider(DbContext);
            var reviews = modelDataProvider.GetReviews(reviewIds.ToArray());

            // get custom vote categories information from database
            var voteCatIds = reviews.Where(x => x.AcceptedReviewVotes?.Count() > 0).SelectMany(x => x.AcceptedReviewVotes.Select(y => y.VoteCategoryId));
            var voteCats = DbContext.Set<VoteCategoryEntity>().Where(x => voteCatIds.Contains(x.CatId)).ToList().Select(x => mapper.MapToVoteCategoryDTO(x));

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
                Voted = reviews.Count(x => x.Comments.Any(y => y.CommentReviewVotes.Count() > 0)),
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
    }
}