using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebSockets;

namespace iRLeagueRESTService.Data
{
    public class StandingsDataProvider : DataProviderBase, IStandingsDataProvider
    {
        public StandingsDataProvider(LeagueDbContext dbContext) : base(dbContext)
        {
        }

        public SeasonStandingsDTO GetStandingsFromSeason(long seasonId, long? sessionId = null)
        {
            // get season entity; get latest season if id == 0
            SeasonEntity season;
            if (seasonId == 0)
            {
                season = DbContext.Set<SeasonEntity>().ToList().OrderByDescending(x => x.SeasonStart).FirstOrDefault();
            }
            else
            {
                season = DbContext.Set<SeasonEntity>().Find(seasonId);
            }

            if (season == null)
            {
                return new SeasonStandingsDTO() { SeasonId = seasonId };
            }

            // get standings from ModelDataProvider
            var scoringTables = season.ScoringTables;
            var modelDataProvider = new ModelDataProvider(DbContext);
            StandingsDataDTO[] standings;
            if (sessionId == null)
            {
                standings = modelDataProvider.GetStandings(scoringTables.Select(x => x.ScoringTableId).ToArray());
            }
            else
            {
                var requestIds = scoringTables.Select(x => new long[] { x.ScoringTableId, sessionId.Value }).ToArray();
                standings = modelDataProvider.GetStandings(requestIds);
            }

            // construct DTO
            var seasonStandings = new SeasonStandingsDTO()
            {
                SeasonId = season.SeasonId,
                Standings = standings
            };

            return seasonStandings;
        }
    }
}