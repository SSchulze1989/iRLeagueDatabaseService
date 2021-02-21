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

        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
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
                    var sessionsDataProvider = new SessionsDataProvider(dbContext);
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

        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
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

        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
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
                if (seasonId == 0)
                {
                    return BadRequestEmptyParameter(nameof(seasonId));
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