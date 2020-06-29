using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Results
{
    public class AddPenaltyEntity : MappableEntity
    {
        [Key, ForeignKey(nameof(ScoredResultRow))]
        public long ScoredResultRowId { get; set; }
        public virtual ScoredResultRowEntity ScoredResultRow { get; set; }
        public int PenaltyPoints { get; set; }
    }
}
