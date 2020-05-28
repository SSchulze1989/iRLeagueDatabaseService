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
using TestConsole.LeagueDBServiceRef;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new LeagueDbContext("TestDatabase");

            var scoring = dbContext.Set<ScoringEntity>().Find(1);

            //var Session = new Session()
            //{
            //    Result = new Result()
            //};
            ////dbContext.Sessions.Add(Session);
            ////dbContext.SaveChanges();
            //Session = dbContext.Sessions.Find(2);

            //var client = new LeagueContext();

            //var session = client.GetModelAsync<SessionModel>(1).Result;


            ILeagueDBService dbService = new LeagueDBServiceClient();

            var dbRequestMsg = new GETItemsRequestMessage()
            {
                databaseName = "TestDatabase",
                userName = "testuser",
                password = "12345",
                requestItemIds = new long[][] { new long[] { 1 } },
                requestItemType = typeof(ScoringDataDTO).FullName,
                requestResponse = true
            };

            var scoringDto = dbService.DatabaseGET(dbRequestMsg).items.First();

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
