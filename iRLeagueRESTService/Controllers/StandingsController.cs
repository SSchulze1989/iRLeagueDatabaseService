using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.DataAccess.Provider;
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
    public class StandingsController : LeagueApiController
    {
        /// <summary>
        /// Logger for this class
        /// </summary>
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(StandingsController));

        /// <summary>
        /// GET Method for getting all standings for a season
        /// If a session id is provided the standings are calculated for all races until this session
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <param name="seasonId">Id of the season</param>
        /// <param name="sessionId">Id of the session (default: <see langword="null"/>)</param>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns>Convenience DTO for season standings</returns>
        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult Get([FromUri] string leagueName, [FromUri] long seasonId = 0, [FromUri] long? sessionId = null, [FromUri] bool includeRaw = false, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Results for session id: {sessionId} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                SeasonStandingsDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    IStandingsDataProvider standingsDataProvider = new StandingsDataProvider(dbContext);
                    data = standingsDataProvider.GetStandingsFromSeason(seasonId, sessionId);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - SeasonStandingsDTO id: {data.SeasonId}");
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
                logger.Error("Error in get Season Standings", e);
                throw e;
            }
        }
    }
}