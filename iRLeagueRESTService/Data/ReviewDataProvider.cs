using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public class ReviewDataProvider : IReviewDataProvider
    {
        private LeagueDbContext DbContext { get; }

        public ReviewDataProvider(LeagueDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ReviewsDTO GetReviewsFromSeason(long seasonId)
        {
            throw new NotImplementedException();
        }

        public ReviewsDTO GetReviewsFromSession(long sessionId)
        {
            // Load session from db
            var session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
            var mapper = new DTOMapper(DbContext);

            var reviewIds = session.Reviews.Select(x => x.ReviewId);

            // get review data from model data provider
            var modelDataProvider = new ModelDataProvider(DbContext);
            var reviews = modelDataProvider.GetReviews(reviewIds.ToArray());
            // get vote categories
            var voteCatIds = reviews.SelectMany(x => x.AcceptedReviewVotes.Select(y => y.VoteCategoryId));
            var voteCats = DbContext.Set<VoteCategoryEntity>().Where(x => voteCatIds.Contains(x.CatId)).ToList().Select(x => mapper.MapToVoteCategoryDTO(x));

            var penalties = reviews
                .SelectMany(x => x.AcceptedReviewVotes)
                .Where(x => x.CatPenalty > 0 && x.MemberAtFaultId != null);
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
            var penaltySummary = new ReviewsPenaltySummaryDTO()
            {
                Count = driverPenalties.Sum(x => x.Count),
                Points = driverPenalties.Sum(x => x.Points),
                DrvPenalties = driverPenalties.ToArray()
            };
            var reviewData = new ReviewsDTO()
            {
                Reviews = reviews,
                Total = reviews.Count(),
                Open = reviews.Count(x => x.AcceptedReviewVotes.Count() == 0),
                Voted = reviews.Count(x => x.Comments.Any(y => y.CommentReviewVotes.Count() > 0)),
                Closed = reviews.Count(x => x.AcceptedReviewVotes.Count() > 0),
                Penalties = penaltySummary,
                //PenDrv = reviews
                //    .SelectMany(x => x.AcceptedReviewVotes)
                //    .Where(x => x.CatPenalty > 0)
                //    .Select(x => x.MemberAtFaultId)
                //    .Distinct()
                //    .Count(),
                //PenPts = reviews.Sum(x => x.AcceptedReviewVotes.Sum(y => y.CatPenalty)),
                Results = reviews
                    .SelectMany(x => x.AcceptedReviewVotes)
                    .Where(x => x.VoteCategoryId != null)
                    .GroupBy(x => x.VoteCategoryId)
                    .Select(x => new CountValue<VoteCategoryDTO>() { Count = x.Count(), Value = voteCats.SingleOrDefault(y => y.CatId == x.Key.Value) })
                    .ToArray(),
                SessionId = sessionId
            };

            return reviewData;
        }
    }
}