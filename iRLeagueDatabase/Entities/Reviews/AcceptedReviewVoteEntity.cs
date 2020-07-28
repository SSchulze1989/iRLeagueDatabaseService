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
    }
}
