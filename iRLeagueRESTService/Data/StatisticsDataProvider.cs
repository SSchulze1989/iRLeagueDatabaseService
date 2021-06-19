using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Statistics;
using iRLeagueDatabase.DataTransfer.Statistics.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Statistics;
using iRLeagueDatabase.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public class StatisticsDataProvider : DataProviderBase, IStatsDataProvider
    {
        public StatisticsDataProvider(LeagueDbContext dbContext) : base(dbContext)
        { }

        public StatisticConvenienceDTO GetStatistics(long seasonId = 0, bool includeSeason = false, bool includeLeague = false)
        {
            var mapper = new DTOMapper(DbContext);

            var seasonStats = new List<SeasonStatsSetConvenienceDTO>();
            var leagueStats = new List<LeagueStatsSetConvenienceDTO>();

            if (includeSeason)
            {
                SeasonEntity season;
                if (seasonId == 0)
                {
                    season = DbContext.Set<SeasonEntity>().OrderByDescending(x => x.SeasonStart).FirstOrDefault();
                }
                else
                {
                    season = DbContext.Seasons.Find(seasonId);
                }

                if (season != null)
                {
                    foreach(var seasonStatistic in season.SeasonStatistics)
                    {
                        var driverStats = seasonStatistic.DriverStatistic
                            .Select(x => mapper.MapToDriverStatisticRowDTO(x))
                            .ToArray();

                        SeasonStatsSetConvenienceDTO currentSet = new SeasonStatsSetConvenienceDTO()
                        {
                            SeasonId = season.SeasonId,
                            SeasonName = season.SeasonName,
                            Id = seasonStatistic.Id,
                            Name = seasonStatistic.Name,
                            Drivers = seasonStatistic.DriverStatistic.Count,
                            DriverStats = driverStats
                        };
                        seasonStats.Add(currentSet);
                    }
                }
            }

            if (includeLeague)
            {
                foreach(var leagueStatistic in DbContext.LeagueStatistics)
                {
                    var driverStats = leagueStatistic.DriverStatistic
                        .Select(x => mapper.MapToDriverStatisticRowDTO(x))
                        .ToArray();

                    LeagueStatsSetConvenienceDTO currentSet = new LeagueStatsSetConvenienceDTO()
                    {
                        Id = leagueStatistic.Id,
                        Name = leagueStatistic.Name,
                        CurrentChamp = mapper.MapToDriverDTO(leagueStatistic.CurrentChamp),
                        Drivers = driverStats.Count(),
                        DriverStats = driverStats
                    };

                    leagueStats.Add(currentSet);
                }
            }

            var statisticDto = new StatisticConvenienceDTO()
            {
                SeasonStats = seasonStats.ToArray(),
                LeagueStats = leagueStats.ToArray()
            };

            return statisticDto;
        }
    }
}