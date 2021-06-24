using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Convenience;
using iRLeagueDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;

namespace iRLeagueDatabase.DataAccess.Provider
{
    /// <summary>
    /// Provides methods to access season data from the provided LeagueDbContext
    /// </summary>
    public class SeasonDataProvider : DataProviderBase, ISeasonDataProvider
    {
        public SeasonDataProvider(LeagueDbContext dbContext) : base(dbContext)
        {

        }

        public SeasonConvenieneDTO GetSeason(long seasonId)
        {
            var lastSeasonId = (DbContext.Set<SeasonEntity>().OrderByDescending(x => x.SeasonStart).FirstOrDefault()?.SeasonId).GetValueOrDefault();
            SeasonEntity season;
            if (seasonId == 0)
            {
                seasonId = lastSeasonId;
            }
            season = DbContext.Set<SeasonEntity>().Find(seasonId);

            if (season == null)
            {
                return null;
            }
            // count sessions and races
            var sessions = season.Schedules.SelectMany(x => x.Sessions);
            var races = sessions.Where(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race);

            // construct DTO
            SeasonConvenieneDTO seasonDTO = new SeasonConvenieneDTO()
            {
                SeasonId = season.SeasonId,
                Name = season.SeasonName,
                IsCurrentSeason = season.SeasonId == lastSeasonId,
                IsFinished = season.Finished,
                SeasonStart = season.SeasonStart,
                SeasonEnd = season.SeasonEnd,
                SessionCount = sessions.Count(),
                RacesCount = races.Count(),
                RacesFinished = races.Count(x => x.SessionResult != null),
            };

            return seasonDTO;
        }

        public SeasonConvenieneDTO[] GetSeasons(params long[] seasonIds)
        {
            IEnumerable<SeasonConvenieneDTO> seasonDTOs;
            if (seasonIds == null || seasonIds.Count() == 0)
            {
                seasonIds = DbContext.Set<SeasonEntity>().Select(x => x.SeasonId).ToArray();
            }

            seasonDTOs = seasonIds.Select(x => GetSeason(x));
            return seasonDTOs.ToArray();
        }
    }
}