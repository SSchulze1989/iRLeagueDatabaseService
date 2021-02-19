using iRLeagueDatabase.DataTransfer.Results.Convenience;
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
    public class ResultsController : LeagueApiController
    {
        /// <summary>
        /// Logger for this class
        /// </summary>
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ReviewsController));

        /// <summary>
        /// GET Method for getting all scored results for a single session
        /// If no session id is provided the last finished session is used instead
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <param name="sessionId">Id of the session (default: null)</param>
        /// <param name="includeRaw">If True, raw results are included</param>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult Get([FromUri] string leagueName, [FromUri] long sessionId = 0, [FromUri] bool includeRaw = false, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
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

                SessionResultsDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    IResultsDataProvider resultsDataProvider = new ResultsDataProvider(dbContext);
                    data = resultsDataProvider.GetResultsFromSession(sessionId, includeRaw);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - ResultsDTO id: {data.SessionId}");
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
                logger.Error("Error in get Results", e);
                throw e;
            }
        }
    }
}