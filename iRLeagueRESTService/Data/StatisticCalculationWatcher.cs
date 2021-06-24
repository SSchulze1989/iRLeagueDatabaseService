using iRLeagueDatabase;
using iRLeagueDatabase.Entities.Statistics;
using iRLeagueManager.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public static class StatisticCalculationWatcher
    {
        private static TimeSpan TickInterval = TimeSpan.FromMinutes(30);

        private static IDictionary<string, Timer> RegisteredWatchers { get; } = new Dictionary<string, Timer>();

        public static void RegisterWatcher(string leagueDbName)
        {
            if (string.IsNullOrEmpty(leagueDbName))
            {
                return;
            }

            if (RegisteredWatchers.ContainsKey(leagueDbName))
            {
                RegisteredWatchers.Remove(leagueDbName);
            }

            var timer = new Timer()
            {
                AutoReset = true,
                Interval = TickInterval.TotalMilliseconds
            };
            timer.Elapsed += async (sender, e) => await Tick(leagueDbName);
            RegisteredWatchers.Add(leagueDbName, timer);
            timer.Start();
            GC.Collect();
        }

        private static async Task Tick(string leagueDbName)
        {
            // Load statistic sets and check for recalculation interval
            using (var dbContext = new LeagueDbContext(leagueDbName))
            {
                var statisticSets = dbContext.Set<StatisticSetEntity>().ToList();
                var checkStatisticSets = statisticSets.Where(x => IsDueTick(x.UpdateTime, TimeSpanConverter.Convert(x.UpdateInterval))).OrderBy(x => GetTypePriority(x));
                
                foreach(var statisticSet in checkStatisticSets)
                {
                    await Calculate(dbContext, statisticSet);
                }
                dbContext.SaveChanges();
            }
            GC.Collect();
        }

        public static async Task Calculate(LeagueDbContext dbContext, StatisticSetEntity statisticSet)
        {
            dbContext.Configuration.LazyLoadingEnabled = false;

            await statisticSet.CheckRequireRecalculationAsync(dbContext);
            if (statisticSet.RequiresRecalculation)
            {
                await statisticSet.LoadRequiredDataAsync(dbContext);
                statisticSet.Calculate(dbContext);
            }

            dbContext.Configuration.LazyLoadingEnabled = true;
        }

        private static int GetTypePriority(object o)
        {
            if (o is SeasonStatisticSetEntity)
            {
                return 1;
            }
            return 999;
        }

        private static bool IsDueTick(DateTime? updateTime, TimeSpan interval)
        {
            if (updateTime == null)
            {
                updateTime = DateTime.MinValue;
            }
            if (interval <= TimeSpan.Zero)
            {
                interval = TimeSpan.FromMinutes(1);
            }

            var now = DateTime.Now;
            var c = (now.Subtract(updateTime.Value)).Ticks / interval.Ticks;

            var nextUpdate = updateTime.Value.Add(TimeSpan.FromTicks(c * interval.Ticks));
            if (now >= nextUpdate && now < nextUpdate.Add(TickInterval))
            {
                return true;
            }

            return false;
        }
    }
}