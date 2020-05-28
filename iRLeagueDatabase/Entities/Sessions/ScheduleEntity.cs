using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Results;

namespace iRLeagueDatabase.Entities.Sessions
{
    public class ScheduleEntity : Revision
    {
        [Key]
        public long ScheduleId { get; set; }

        public string Name { get; set; }

        public override object MappingId => ScheduleId;

        public SeasonEntity Season { get; set; }

        public DateTime? ScheduleStart => Sessions.Select(x => x.Date).Min();

        public DateTime? ScheduleEnd => Sessions.Select(x => x.Date).Max();

        //[InverseProperty(nameof(SessionBaseEntity.Schedule))]
        public virtual List<SessionBaseEntity> Sessions { get; set; } = new List<SessionBaseEntity>();

        public virtual ScoringEntity ConnectedScoring { get; set; }

        public ScheduleEntity() { }

        public ScheduleEntity(int scheduleId) : this()
        {
            ScheduleId = scheduleId;
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            Sessions.ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }
}
