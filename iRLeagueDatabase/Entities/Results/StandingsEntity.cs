using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Results
{
    public class StandingsEntity : MappableEntity
    {
        public virtual List<StandingsRowEntity> StandingsRows { get; set; }
        public virtual LeagueMemberEntity MostWinsDriver { get; set; }
        public virtual LeagueMemberEntity MostPolesDriver { get; set; }
        public virtual LeagueMemberEntity CleanestDriver { get; set; }
        public virtual LeagueMemberEntity MostPenaltiesDriver { get; set; }

        public StandingsEntity()
        {
            StandingsRows = new List<StandingsRowEntity>();
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            throw new InvalidOperationException("Could not delete " + GetType().Name + ". Entity is not attached to database.");
        }
    }
}
