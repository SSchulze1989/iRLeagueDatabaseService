using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Results
{
    public class TeamStandingsRowEntity : StandingsRowEntity
    {
        public TeamEntity Team { get; set; }
        public virtual List<StandingsRowEntity> DriverStandingsRows { get; set; }
    }
}
