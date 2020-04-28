using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using iRLeagueDatabase;
//using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.Mapper;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var dbContext = new TestDbContext();

            //var Session = new Session()
            //{
            //    Result = new Result()
            //};
            ////dbContext.Sessions.Add(Session);
            ////dbContext.SaveChanges();
            //Session = dbContext.Sessions.Find(2);

            //var client = new LeagueContext();

            //var session = client.GetModelAsync<SessionModel>(1).Result;

            //Console.ReadKey();

            var dbContext = new LeagueDbContext();

            var season = dbContext.Set<SeasonEntity>().First();
            var mapper = new DTOMapper();

            SeasonDataDTO seasonDto = mapper.MapTo(season, typeof(SeasonDataDTO)) as SeasonDataDTO;


        }

        static void NotifyDirect(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName + " changed. (direct)");
        }

        static void NotifyContainer(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName + " changed. (container)");
        }
    }
}
