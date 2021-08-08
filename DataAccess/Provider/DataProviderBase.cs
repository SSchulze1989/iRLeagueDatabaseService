using iRLeagueDatabase;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public class DataProviderBase
    {
        protected IProviderContext<LeagueDbContext> ProviderContext { get; }
        protected LeagueDbContext DbContext => ProviderContext.ModelStore;

        public string UserName => ProviderContext.UserName;
        public string UserId => ProviderContext.UserId;
        public LeagueRoleEnum LeagueRoles => ProviderContext.LeagueRoles;

        public DataProviderBase(IProviderContext<LeagueDbContext> context)
        {
            ProviderContext = context;
        }

        /// <summary>
        /// Check if the provided entity belongs to the required league
        /// </summary>
        /// <param name="leagueId">Id of the league the entity must belong to</param>
        /// <param name="entity">Entity to check</param>
        /// <returns><see langword="true"/> if the entity belongs to the league</returns>
        protected bool CheckLeague(long leagueId, IHasLeagueId entity)
        {
            bool result = false;
            var rememberLazyLoading = DbContext.Configuration.LazyLoadingEnabled;
            DbContext.Configuration.LazyLoadingEnabled = true;
            if (entity.GetLeagueId() == DbContext.CurrentLeagueId)
            {
                result = true;
            }
            DbContext.Configuration.LazyLoadingEnabled = rememberLazyLoading;
            return result;
        }
    }
}