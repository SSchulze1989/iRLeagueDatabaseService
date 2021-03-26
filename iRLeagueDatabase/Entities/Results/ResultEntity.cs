using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Reviews;


namespace iRLeagueDatabase.Entities.Results
{
    public class ResultEntity : Revision
    {[Key, ForeignKey(nameof(Session))]
        public virtual long ResultId { get; set; }
        [Required]
        public virtual SessionBaseEntity Session { get; set; }

        [InverseProperty(nameof(IRSimSessionDetailsEntity.Result))]
        public virtual IRSimSessionDetailsEntity IRSimSessionDetails { get; set; }

        public override object MappingId => ResultId;

        [ForeignKey(nameof(Season))]
        public long? SeasonId { get; set; }
        public virtual SeasonEntity Season { get; set; }

        /// <summary>
        /// Data rows of results table
        /// This contains all result rows including practice and qualy together. They can be distinguished by SimSessionType value
        /// </summary>
        [InverseProperty(nameof(ResultRowEntity.Result))]
        public virtual List<ResultRowEntity> RawResults { get; set; }

        [NotMapped]
        public List<IncidentReviewEntity> Reviews => Session?.Reviews;

        /// <summary>
        /// All scored Results that were calculated based on this Result set.
        /// </summary>
        [InverseProperty(nameof(ScoredResultEntity.Result))]
        public virtual List<ScoredResultEntity> ScoredResults { get; set; }

        public long PoleLaptime { get; set; }

        public bool RequiresRecalculation { get; set; }

        [NotMapped]
        public IEnumerable<Members.LeagueMemberEntity> DriverList => RawResults.Select(x => x.Member).Distinct();

        //public List<ResultRow> FinalResults { get; set; }

        public ResultEntity()
        {
            RawResults = new List<ResultRowEntity>();
            //FinalResults = new List<ResultRow>();
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            IRSimSessionDetails?.Delete(dbContext);
            RawResults?.ToList().ForEach(x => x.Delete(dbContext));
            ScoredResults?.ToList().ForEach(x =>  x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }
}
