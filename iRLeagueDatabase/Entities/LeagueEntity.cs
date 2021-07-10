using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Statistics;

namespace iRLeagueDatabase.Entities
{
    public class LeagueEntity : Revision, IHasLeagueId
    {
        [Key]
        public long LeagueId { get; set; }
        public override object MappingId => LeagueId;

        public string LeagueName { get; set; }

        public string LongName { get; set; }

        // League settings:
        public bool IsPublic { get; set; }

        public bool IsActive { get; set; }


        [InverseProperty(nameof(SeasonEntity.League))]
        public virtual List<SeasonEntity> Seasons { get; set; }
        [InverseProperty(nameof(CustomIncidentEntity.League))]
        public virtual List<CustomIncidentEntity> CustomIncidents { get; set; }
        [InverseProperty(nameof(StatisticSetEntity.League))]
        public virtual List<StatisticSetEntity> StatisticSets { get; set; }
        [InverseProperty(nameof(LeagueMemberEntity.League))]
        public virtual List<LeagueMemberEntity> LeagueMembers { get; set; }
        [InverseProperty(nameof(VoteCategoryEntity.League))]
        public virtual List<VoteCategoryEntity> VoteCategories { get; set; }

        public long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
