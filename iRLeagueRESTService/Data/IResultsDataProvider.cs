using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueRESTService.Data
{
    public interface IResultsDataProvider
    {
        /// <summary>
        /// Get the results of a single session
        /// </summary>
        /// <param name="sessionId">Id of the session</param>
        /// <param name="includeRawResults">If <see langword="true"/> raw result data is included</param>
        /// <returns>Convenience DTO for all session results</returns>
        SessionResultsDTO GetResultsFromSession(long sessionId, bool includeRawResults = false, ScoredResultDataDTO[] scoredResults = null);
        /// <summary>
        /// Get the results of a whole season
        /// </summary>
        /// <param name="seasonId">Id of the season</param>
        /// <param name="includeRawResults">If <see langword="true"/> raw result data is included</param>
        /// <returns>Convenience DTO for all season results</returns>
        SeasonResultsDTO GetResultsFromSeason(long seasonId, bool includeRawResults = false);
    }
}
