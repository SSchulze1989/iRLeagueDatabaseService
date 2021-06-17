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
        public abstract long LeagueId { get; set; }
        public abstract LeagueEntity League { get; set; }

        public long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
