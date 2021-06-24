using iRLeagueDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public class DataProviderBase
    {
        protected LeagueDbContext DbContext { get; }

        public string UserName { get; set; }
        public string UserId { get; set; }

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

        public DataProviderBase(LeagueDbContext context, string userName, string userId) : this(context, userName)
        {
            UserId = userId;
        }
    }
}