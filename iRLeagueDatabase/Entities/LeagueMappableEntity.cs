using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities
{
    public abstract class LeagueMappableEntity : MappableEntity, IHasLeagueId
    {
        [ForeignKey(nameof(League))]
        public long LeagueId { get; set; }
        public virtual LeagueEntity League { get; set; }

        public abstract long GetLeagueId();
    }
}
