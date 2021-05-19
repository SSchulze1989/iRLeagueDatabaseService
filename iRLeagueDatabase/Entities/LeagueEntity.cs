using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities
{
    public class LeagueEntity : Revision
    {
        [Key]
        public long LeagueId { get; set; }
        public override object MappingId => LeagueId;

        [InverseProperty(nameof(SeasonEntity.League))]
        public virtual List<SeasonEntity> Seasons { get; set; }

        public override long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
