using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Results;

namespace iRLeagueDatabase.Entities.Reviews
{
    public class ReviewPenaltyEntity : LeagueMappableEntity
    {
        [Key, ForeignKey(nameof(ScoredResultRow)), Column(Order = 0)]
        public long ResultRowId { get; set; }
        public virtual ScoredResultRowEntity ScoredResultRow { get; set; }
        
        [Key, ForeignKey(nameof(Review)), Column(Order = 1)]
        public long ReviewId { get; set; }
        public virtual IncidentReviewEntity Review { get; set; }

        [ForeignKey(nameof(ReviewVote))]
        public long? ReviewVoteId { get; set; }
        public virtual AcceptedReviewVoteEntity ReviewVote { get; set; }

        public int PenaltyPoints { get; set; }

        public override object MappingId => new { ResultRowId, ReviewId };

        public override void Delete(LeagueDbContext dbContext)
        {
            base.Delete(dbContext);
        }

        public override long GetLeagueId()
        {
            return ScoredResultRow.GetLeagueId();
        }
    }
}
