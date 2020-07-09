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
using iRLeagueDatabase.Entities.Members;
using System.Net.Http;
using iRLeagueDatabase.DataTransfer.Messages;


namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new LeagueDbContext("TestDatabase"))
            {
                var user = context.Users.First();

                Console.WriteLine("Passwort:");
                string pw = null;

                while (pw != "x")
                {
                    pw = Console.ReadLine();
                    var pwBytes = Encoding.UTF8.GetBytes(pw);

                    Console.WriteLine(user.CheckCredentials(pwBytes));
                }
            }

            Console.Read();


                //using (var dbContext = new LeagueDbContext("TestDatabase"))
                ////{

                //var init = dbContext.Set<SeasonEntity>().First();

                //var test = dbContext.Set<ScoredResultEntity>()
                //    .Include(x => x.Result)
                //    .Include(x => x.Result.Session)
                //    .Include(x => x.Result.Session.Schedule.Season)
                //    .Include(x => x.FastestAvgLapDriver)
                //    .Include(x => x.FastestLapDriver)
                //    .Include(x => x.FastestQualyLapDriver)
                //    .Include(x => x.Result.RawResults)
                //    .Include(x => x.Result.RawResults.Select(y => y.Member))
                //    //.Include(x => x.Scoring)
                //    .Include(x => x.FinalResults)
                //    .Include(x => x.FinalResults.Select(y => y.ResultRow))
                //    .Include(x => x.FinalResults.Select(y => y.ResultRow.Member))
                //    .First();

                //var scoringId = 3;
                //dbContext.Configuration.LazyLoadingEnabled = false;

                //var scoredResults = dbContext.Set<ScoredResultEntity>().Where(x => x.ScoringId == scoringId);
                //scoredResults.Load();
                //var scoredResultRows = dbContext.Set<ScoredResultRowEntity>().Where(x => scoredResults.Any(y => x.ScoredResultId == y.ResultId && y.ScoringId == y.ScoringId));
                //scoredResultRows.Load();
                //var results = dbContext.Set<ResultEntity>().Where(x => scoredResults.Any(y => y.ResultId == x.ResultId));
                //results.Load();
                //var resultRows = dbContext.Set<ResultRowEntity>().Where(x => results.Any(y => y.ResultId == x.ResultId));
                //resultRows.Load();
                //dbContext.Set<LeagueMemberEntity>().Where(x => resultRows.Any(y => y.MemberId == x.MemberId)).Load();
                //dbContext.Set<SessionBaseEntity>().Load();
                //dbContext.Set<ScoringEntity>();
                //dbContext.Seasons.Load();

                //var standingsTest = dbContext.Set<ScoringEntity>()
                //            .Include(x => x.Sessions)
                //            .Include(x => x.ScoredResults.Select(y => y.FinalResults.Select(z => z.ResultRow.Member)))
                //            .Include(x => x.MultiScoringResults)
                //            .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults))
                //            .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Result.Session.Schedule.Season))))
                //            .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Member))))
                //            .FirstOrDefault(x => x.ScoringId == scoringId);

                //    var mapper = new DTOMapper();

                //    //var testDto = mapper.MapTo<ScoredResultDataDTO>(test);

                //    //testDto = mapper.MapTo<ScoredResultDataDTO>(test);

                //    var testStanding = standingsTest.GetSeasonStandings();

                //    testStanding = standingsTest.GetSeasonStandings();

                //    var testStandingDto = mapper.MapTo<StandingsDataDTO>(testStanding);

                //    Console.Read();
                //}
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

                //Console.ReadKey();

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
