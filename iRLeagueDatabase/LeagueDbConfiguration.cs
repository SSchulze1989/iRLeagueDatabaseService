using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.IO;

namespace iRLeagueDatabase
{
    public class LeagueDbConfiguration : DbConfiguration
    {
        private readonly string DbModelStorePath = "C:\\iRLeagueDatabaseService\\DbModelStore";

        public LeagueDbConfiguration() : base()
        {
            //var path = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var path = DbModelStorePath;
            SetModelStore(new DefaultDbModelStore(path));
        }
    }
}
