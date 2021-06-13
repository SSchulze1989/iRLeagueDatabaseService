using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Sessions.Convenience;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace iRLeagueRESTService.Controllers
{
    /// <summary>
    /// API endpoint to enable easy access to Sessions data for either a single session, a schedule or a whole season
    /// Data will span some simple summarized statistics and the complete sessions data
    /// 
    /// This endpoint only provides GET Method
    /// </summary>
    public class SessionsController : LeagueApiController
    {
        /// <summary>
        /// Logger for this class
        /// </summary>
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ReviewsController));

        /// <summary>
        /// GET Method for getting all basic infos for a single session
        /// If no session id is provided the last finished session is used instead
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <param name="sessionId">Id of the session (default: <see langword="null"/>)</param>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns><see cref="SessionDataDTO"/> of the session</returns>
        [HttpGet]
        [LeagueAuthorize(Roles = iRLeagueDatabase.Enums.LeagueRoleEnum.User)]
        public IHttpActionResult GetSession([FromUri] string leagueName, [FromUri] long sessionId = 0, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Sessions for session id: {sessionId} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                // Get session data from data access layer
                SessionDataDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    ISessionsDataProvider sessionsDataProvider = new SessionsDataProvider(dbContext);
                    data = sessionsDataProvider.GetSession(sessionId);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - SessionDataDTO id: {data.SessionId}");
                if (string.IsNullOrEmpty(fields))
                {
                    return Ok(data);
                }
                else
                {
                    data.SetSerializableProperties(fields.Split(','), excludeFields);
                    var response = SelectFieldsHelper.GetSelectedFieldObject(data);
                    return Json(response);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in get Sessions", e);
                throw e;
            }
        }

        /// <summary>
        /// GET Method for getting all basic infos for all sessions of a schedule
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <param name="scheduleId">Id of the schedule</param>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns><see cref="ScheduleSessionsDTO"/> of the schedule</returns>
        [HttpGet]
        [LeagueAuthorize(Roles = iRLeagueDatabase.Enums.LeagueRoleEnum.User)]
        public IHttpActionResult GetSchedule([FromUri] string leagueName, [FromUri] long scheduleId, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Sessions for schedule id: {scheduleId} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }
                if (scheduleId == 0)
                {
                    return BadRequestEmptyParameter(nameof(scheduleId));
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                // Get session data from data access layer
                ScheduleSessionsDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    var sessionsDataProvider = new SessionsDataProvider(dbContext);
                    data = sessionsDataProvider.GetSessionsFromSchedule(scheduleId);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - {nameof(ScheduleSessionsDTO)} id: {data.ScheduleId}");
                if (string.IsNullOrEmpty(fields))
                {
                    return Ok(data);
                }
                else
                {
                    data.SetSerializableProperties(fields.Split(','), excludeFields);
                    var response = SelectFieldsHelper.GetSelectedFieldObject(data);
                    return Json(response);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in get Sessions", e);
                throw e;
            }
        }

        /// <summary>
        /// GET Method for getting all basic infos for all sessions of a season
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <param name="seasonId">Id of the session</param>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns><see cref="SeasonSessionsDTO"/> of the season</returns>
        [HttpGet]
        [LeagueAuthorize(Roles = iRLeagueDatabase.Enums.LeagueRoleEnum.User)]
        public IHttpActionResult GetSeason([FromUri] string leagueName, [FromUri] long seasonId, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Sessions for season id: {seasonId} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                // Get session data from data access layer
                SeasonSessionsDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    var sessionsDataProvider = new SessionsDataProvider(dbContext);
                    data = sessionsDataProvider.GetSessionsFromSeason(seasonId);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - {nameof(SeasonSessionsDTO)} id: {data?.SeasonId}");
                if (string.IsNullOrEmpty(fields))
                {
                    return Ok(data);
                }
                else
                {
                    data.SetSerializableProperties(fields.Split(','), excludeFields);
                    var response = SelectFieldsHelper.GetSelectedFieldObject(data);
                    return Json(response);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in get Sessions", e);
                throw e;
            }
        }
    }
}