using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Sessions;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoredResultEntity : MappableEntity
    {
        [Key, Column(Order = 0)]
        public long ResultId { get; set; }
        [ForeignKey(nameof(ResultId))]
        public virtual ResultEntity Result { get; set; }
        [Key, Column(Order = 1)]
        public long ScoringId { get; set; }
        [ForeignKey(nameof(ScoringId))]
        public virtual ScoringEntity Scoring { get; set; }
        public virtual LeagueMemberEntity FastestLapDriver { get; set; }
        public long FastestLap { get; set; }
        public virtual LeagueMemberEntity FastestQualyLapDriver { get; set; }
        public long FastestQualyLap { get; set; }
        public virtual LeagueMemberEntity FastestAvgLapDriver { get; set; }
        public long FastestAvgLap { get; set; }
        [InverseProperty(nameof(ScoredResultRowEntity.ScoredResult))]
        public virtual List<ScoredResultRowEntity> FinalResults { get; set; } = new List<ScoredResultRowEntity>();

        public override void Delete(LeagueDbContext dbContext)
        {
            FinalResults.ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }
}
