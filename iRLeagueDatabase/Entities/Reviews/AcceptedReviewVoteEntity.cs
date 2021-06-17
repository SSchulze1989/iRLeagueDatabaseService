using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Reviews
{
    public class AcceptedReviewVoteEntity : ReviewVoteEntity
    {
        [ForeignKey(nameof(IncidentReview))]
        public long ReviewId { get; set; }
        public virtual IncidentReviewEntity IncidentReview { get; set; }

        [ForeignKey(nameof(League))]
        public override long LeagueId { get; set; }
        public override LeagueEntity League { get; set; }

        [InverseProperty(nameof(ReviewPenaltyEntity.ReviewVote))]
        public virtual List<ReviewPenaltyEntity> ReviewPenalties { get; set; }

        public override void Delete(LeagueDbContext dbContext)
        {
            IncidentReview?.AcceptedReviewVotes?.Remove(this);
            ReviewPenalties?.ForEach(x => x.ReviewVote = null);
            if (IncidentReview?.Session?.SessionResult != null)
            {
                IncidentReview.Session.SessionResult.RequiresRecalculation = true;
            }
            base.Delete(dbContext);
        }
    }
}
