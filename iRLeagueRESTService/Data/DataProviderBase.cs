using iRLeagueDatabase;
using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Data
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
    }
}