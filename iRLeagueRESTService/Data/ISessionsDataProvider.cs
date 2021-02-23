using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Sessions.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueRESTService.Data
{
    public interface ISessionsDataProvider
    {
        /// <summary>
        /// Get session data for a single session
        /// </summary>
        /// <param name="sessionId">Id of the session</param>
        /// <param name="includeScheduleData">If <see langword="true"/>: schedule id and name are included</param>
        /// <returns>Convenience DTO for the session data; <see langword="null"/> if sessionId not found</returns>
        SessionDataDTO GetSession(long sessionId, bool includeScheduleData = true);
        /// <summary>
        /// Get session data for a single schedule
        /// </summary>
        /// <param name="scheduleId">Id of the schedule</param>
        /// <returns>Convenience DTO for the sessions data</returns>
        ScheduleSessionsDTO GetSessionsFromSchedule(long scheduleId);
        /// <summary>
        /// Get session data for a whole season
        /// </summary>
        /// <param name="seasonId">Id of the season</param>
        /// <returns>Convenience DTO for the sessions data</returns>
        SeasonSessionsDTO GetSessionsFromSeason(long seasonId);
    }
}
