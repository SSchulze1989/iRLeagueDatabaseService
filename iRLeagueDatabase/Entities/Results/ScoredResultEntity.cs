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
    public class ScoredResultEntity : Revision
    {
        //[Key]
        //public long ScoredResultId { get; set; }
        [NotMapped]
        public override object MappingId => new { ResultId, ScoringId };
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
        public virtual List<LeagueMemberEntity> HardChargers { get; set; }
        public virtual List<LeagueMemberEntity> CleanestDrivers { get; set; }

        [InverseProperty(nameof(ScoredResultRowEntity.ScoredResult))]
        public virtual List<ScoredResultRowEntity> FinalResults { get; set; }

        public ScoredResultEntity()
        {
            FinalResults = new List<ScoredResultRowEntity>();
            HardChargers = new List<LeagueMemberEntity>();
            CleanestDrivers = new List<LeagueMemberEntity>();
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            FinalResults?.ToList().ForEach(x => x.Delete(dbContext));
            HardChargers.Clear();
            CleanestDrivers.Clear();
            base.Delete(dbContext);
        }
    }
}
