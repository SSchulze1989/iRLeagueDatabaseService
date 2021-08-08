using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public class ProviderContext<TModelStore> : IProviderContext<TModelStore>
    {
        public ProviderContext(TModelStore modelStore, long leagueId, string userName, string userId, LeagueRoleEnum leagueRoles)
        {
            ModelStore = modelStore;
            LeagueId = leagueId;
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            LeagueRoles = leagueRoles;
        }

        public TModelStore ModelStore { get; }

        public long LeagueId { get; }

        public string UserName { get; }

        public string UserId { get; }

        public LeagueRoleEnum LeagueRoles { get; }
    }
}
