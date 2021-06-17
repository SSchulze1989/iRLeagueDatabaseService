using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Results;

namespace iRLeagueDatabase.Entities.Members
{
    public class TeamEntity : LeagueRevision
    {
        [Key]
        public long TeamId { get; set; }
        public override object MappingId => TeamId;

        [ForeignKey(nameof(League))]
        public override long LeagueId { get; set; }
        public override LeagueEntity League { get; set; }

        [InverseProperty(nameof(LeagueMemberEntity.Team))]
        public virtual List<LeagueMemberEntity> Members { get; set; }
        [InverseProperty(nameof(ScoredTeamResultRowEntity.Team))]
        public virtual List<ScoredTeamResultRowEntity> TeamResultRows { get; set; }

        public string Name { get; set; }
        public string Profile { get; set; }
        public string TeamColor { get; set; }
        public string TeamHomepage { get; set; }

        public override void Delete(LeagueDbContext dbContext)
        {
            Members?.ToList().ForEach(x => x.Team = null);
            TeamResultRows?.ToList().ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }
}
