using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueRESTService.Data
{
    public interface IStandingsDataProvider
    {
        /// <summary>
        /// Get all available standings for a season
        /// </summary>
        /// <param name="seasonId">Id of the season</param>
        /// <param name="sessionId">Id of the last counted session; If <see langword="null"/> return last active season</param>
        /// <returns>Convenience DTO for season standings</returns>
        SeasonStandingsDTO GetStandingsFromSeason(long seasonId, long? sessionId = null);
    }
}
