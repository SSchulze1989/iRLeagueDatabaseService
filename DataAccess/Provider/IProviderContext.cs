using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public interface IProviderContext<TModelStore>
    {
        TModelStore ModelStore { get; }
        long LeagueId { get; }

        string UserName { get; }
        string UserId { get; }
        LeagueRoleEnum LeagueRoles { get; }
    }
}
