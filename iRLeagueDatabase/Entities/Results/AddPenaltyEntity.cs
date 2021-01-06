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

        public override object MappingId => ScoredResultRowId;

        public override void Delete(LeagueDbContext dbContext)
        {
            if (ScoredResultRow?.ScoredResult?.Result != null)
            {
                ScoredResultRow.ScoredResult.Result.RequiresRecalculation = true;
            }
            base.Delete(dbContext);
        }
    }
}
