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
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.Mapper;
//using TestConsole.LeagueDBServiceRef;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbContext = new LeagueDbContext("TestDatabase"))
            {

                var init = dbContext.Set<SeasonEntity>().First();

                var test = dbContext.Set<ScoredResultEntity>().ToList();

                var mapper = new DTOMapper();

                var testDto = mapper.MapTo<ScoredResultDataDTO>(test.First());

                Console.Read();
            }
            //var scoring = dbContext.Set<ScoringEntity>().Find(1);

            //var test = scoring.GetSeasonStandings();

            //var Session = new Session()
            //{
            //    Result = new Result()
            //};
            ////dbContext.Sessions.Add(Session);
            ////dbContext.SaveChanges();
            //Session = dbContext.Sessions.Find(2);

            //var client = new LeagueContext();

            //var session = client.GetModelAsync<SessionModel>(1).Result;


            //ILeagueDBService dbService = new LeagueDBServiceClient();

            //var dbService = new LeagueDBService.LeagueDBService();

            //var dbRequestMsg = new iRLeagueDatabase.DataTransfer.Messages.GETItemsRequestMessage()
            //{
            //    databaseName = "TestDatabase",
            //    userName = "testuser",
            //    password = "12345",
            //    requestItemIds = new long[][] { new long[] { 1 } },
            //    requestItemType = typeof(iRLeagueDatabase.DataTransfer.Results.StandingsDataDTO).Name,
            //    requestResponse = true
            //};

            Console.ReadKey();

            //var response = dbService.GetFromDatabase(dbRequestMsg);
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
