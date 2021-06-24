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
        protected LeagueDbContext DbContext { get; }

        public string UserName { get; }
        public string UserId { get; }
        public LeagueRoleEnum LeagueRoles { get; }

        public DataProviderBase()
        {
            DbContext = new LeagueDbContext();
        }

        public DataProviderBase(LeagueDbContext context)
        {
            DbContext = context;
        }

        public DataProviderBase(LeagueDbContext context, string userName) : this(context)
        {
            UserName = userName;
        }

        public DataProviderBase(LeagueDbContext context, string userName, string userId, LeagueRoleEnum roles) : this(context, userName)
        {
            UserId = userId;
            LeagueRoles = roles;
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