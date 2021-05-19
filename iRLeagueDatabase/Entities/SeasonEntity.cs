using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Statistics;

namespace iRLeagueDatabase.Entities
{
    public class SeasonEntity : Revision
    {
        [Key]
        public long SeasonId { get; set; }

        public override object MappingId => SeasonId;

        [ForeignKey(nameof(League))]
        public long LeagueId { get; set; }
        public virtual LeagueEntity League { get; set; }

        public string SeasonName { get; set; }

        public bool Finished { get; set; }
        
        public virtual List<ScheduleEntity> Schedules { get; set; }

        public virtual List<ScoringEntity> Scorings { get; set; }

        public virtual List<ScoringTableEntity> ScoringTables { get; set; }

        [InverseProperty(nameof(SeasonStatisticSetEntity.Season))]
        public virtual List<SeasonStatisticSetEntity> SeasonStatistics { get; set; }

        //public virtual List<IncidentReviewEntity> Reviews { get; set; }

        [InverseProperty(nameof(ResultEntity.Season))]
        public virtual IEnumerable<ResultEntity> Results { get; set; }

        //[ForeignKey(nameof(MainScoring))]
        //public int? MainScoringId { get; set; }
        public ScoringEntity MainScoring { get; set; }

        public DateTime? SeasonStart { get => Schedules.Select(x => x.ScheduleStart).Min(); set { } }
        public DateTime? SeasonEnd { get => Schedules.Select(x => x.ScheduleEnd)?.Max(); set { } }

        public bool HideCommentsBeforeVoted { get; set; }

        public SeasonEntity()
        {
            Schedules = new List<ScheduleEntity>();
            //Scorings = new List<ScoringEntity>();
            //Reviews = new List<IncidentReviewEntity>();
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            Schedules?.ToList().ForEach(x => x.Delete(dbContext));
            Scorings?.ToList().ForEach(x => x.Delete(dbContext));
            ScoringTables?.ToList().ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }

        public override long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
