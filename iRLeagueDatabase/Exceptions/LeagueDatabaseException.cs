using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Exceptions
{
    public class LeagueDatabaseException : Exception
    {
        public LeagueDbContext DbContext { get; }
        public string DatabaseName { get; }

        public LeagueDatabaseException(string message, LeagueDbContext dbContext) : this(message, dbContext, null)
        {

        }

        public LeagueDatabaseException(string message, LeagueDbContext dbContext, Exception innerException) : base(message, innerException)
        {
            DbContext = dbContext;
            DatabaseName = DbContext.Database.Connection.Database;
        }
    }
}
