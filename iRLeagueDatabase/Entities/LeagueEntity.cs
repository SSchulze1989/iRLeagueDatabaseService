using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities
{
    public class LeagueEntity : LeagueRevision
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

        public override long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
