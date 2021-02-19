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
        /// <returns>Convenience DTO for all session results</returns>
        SessionResultsDTO GetResultsFromSession(long sessionId, bool includeRawResults = false);
    }
}
