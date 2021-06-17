using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities
{
    public interface IHasLeagueId
    {
        long GetLeagueId();
    }
}
