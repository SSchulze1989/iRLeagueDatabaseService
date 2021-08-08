using iRLeagueDatabase.DataAccess.Provider.Generic;
using iRLeagueDatabase;
using iRLeagueDatabase.DataAccess.Mapper;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public class StandingsDataProvider : DataProviderBase, IStandingsDataProvider
    {
        public StandingsDataProvider(IProviderContext<LeagueDbContext> context) : base(context)
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
            IDataProvider<StandingsDataDTO, long[]> genericDataProvider = new GenericStandingsDataProvider(ProviderContext);
            StandingsDataDTO[] standings;
            if (sessionId == null)
            {
                var requestIds = scoringTables.Select(x => new long[] { x.ScoringTableId });
                standings = genericDataProvider.GetData(requestIds).ToArray();
            }
            else
            {
                var requestIds = scoringTables.Select(x => new long[] { x.ScoringTableId, sessionId.Value });
                standings = genericDataProvider.GetData(requestIds).ToArray();
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