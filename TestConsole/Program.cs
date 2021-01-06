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
//using TestConsole.LeagueDBServiceRef;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.Entities.Members;
using System.Net.Http;
using System.Security.Principal;
using iRLeagueDatabase.Extensions;
using iRLeagueDatabase.Entities.Statistics;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create season statistic set
            //using (var dbContext = new LeagueDbContext("SkippyCup_leagueDb"))
            //{
            //    var season = dbContext.Seasons.Find(1);

            //    var seasonStatistics = new SeasonStatisticSetEntity();
            //    season.SeasonStatistics.Add(seasonStatistics);

            //    var sprintScoring = dbContext.Set<ScoringEntity>().Find(7);
            //    var enduranceScoring = dbContext.Set<ScoringEntity>().Find(8);

            //    seasonStatistics.Scorings.Add(sprintScoring);
            //    seasonStatistics.Scorings.Add(enduranceScoring);

            //    dbContext.SaveChanges();
            //}

            // Calculate season statistics
            using (var dbContext = new LeagueDbContext("SkippyCup_leagueDb"))
            {
                var seasonStatistics = dbContext.Set<SeasonStatisticSetEntity>().ToList();

                dbContext.Configuration.LazyLoadingEnabled = false;

                seasonStatistics.ForEach(x => x.LoadRequiredDataAsync(dbContext).Wait());
                seasonStatistics.ForEach(x => x.Calculate(dbContext));

                dbContext.SaveChanges();
            }

            // Create league statistic set
            //using (var dbContext = new LeagueDbContext("SkippyCup_leagueDb"))
            //{
            //    var leagueStatistic = new LeagueStatisticSetEntity();
            //    dbContext.LeagueStatistics.Add(leagueStatistic);

            //    leagueStatistic.StatisticSets.Add(dbContext.Seasons.First().SeasonStatistics.First());
            //    leagueStatistic.StatisticSets.Add(dbContext.Seasons.First().SeasonStatistics.Last());

            //    dbContext.SaveChanges();
            //}

            // Calculate league statistics
            using (var dbContext = new LeagueDbContext("SkippyCup_leagueDb"))
            {
                var leagueStatistic = dbContext.Set<LeagueStatisticSetEntity>().First();

                dbContext.Configuration.LazyLoadingEnabled = false;

                leagueStatistic.LoadRequiredDataAsync(dbContext).Wait();
                leagueStatistic.Calculate(dbContext);

                dbContext.SaveChanges();
            }

            //Console.Read();
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
