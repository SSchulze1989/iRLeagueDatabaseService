using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Interfaces
{
    public interface ILeagueInfo
    {
        long LeagueId { get; }
        string LeagueName { get; }
    }
}
