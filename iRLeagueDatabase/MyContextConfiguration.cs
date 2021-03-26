using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase
{
    public class MyContextConfiguration : DbConfiguration
    {
        public static readonly string DbModelStorePath = "C:\\iRLeagueDatabaseService\\DbModelStore";
        
        public MyContextConfiguration()
        {
            MyDbModelStore cachedDbModelStore = new MyDbModelStore(DbModelStorePath);
            IDbDependencyResolver dependencyResolver = new SingletonDependencyResolver<DbModelStore>(cachedDbModelStore);
            AddDependencyResolver(dependencyResolver);
        }

        private class MyDbModelStore : DefaultDbModelStore
        {
            public MyDbModelStore(string location)
                : base(location)
            { }

            public override DbCompiledModel TryLoad(Type contextType)
            {
                string path = GetFilePath(contextType);
                if (File.Exists(path))
                {
                    DateTime lastWriteTime = File.GetLastWriteTimeUtc(path);
                    DateTime lastWriteTimeDomainAssembly = File.GetLastWriteTimeUtc(typeof(LeagueDbContext).Assembly.Location);
                    if (lastWriteTimeDomainAssembly > lastWriteTime)
                    {
                        File.Delete(path);
                        //Tracers.EntityFramework.TraceInformation("Cached db model obsolete. Re-creating cached db model edmx.");
                    }
                }
                else
                {
                    //Tracers.EntityFramework.TraceInformation("No cached db model found. Creating cached db model edmx.");
                }

                return base.TryLoad(contextType);
            }
        }
    }
}
