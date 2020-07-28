using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Reviews;

namespace iRLeagueDatabase.Entities.Results
{
    public class ReviewPenaltyEntity : MappableEntity
    {
        [Key, ForeignKey(nameof(ScoredResultRow)), Column(Order = 0)]
        public long ResultRowId { get; set; }
        public virtual ScoredResultRowEntity ScoredResultRow { get; set; }

        [Key, ForeignKey(nameof(Review)), Column(Order = 1)]
        public long ReviewId { get; set; }
        public virtual IncidentReviewEntity Review { get; set; }

        public int PenaltyPoints { get; set; }
    }
}
