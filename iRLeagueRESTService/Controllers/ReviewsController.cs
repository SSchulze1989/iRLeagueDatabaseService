using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Reviews.Convenience;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Http;

namespace iRLeagueRESTService.Controllers
{
    /// <summary>
    /// API endpoint to enable easy access to Reviews data for either a single session or a whole season
    /// Data will span some simple summarized statistics and the complete reviews data including comments and votes
    /// 
    /// This endpoint only provides GET Method
    /// </summary>
    public class ReviewsController : LeagueApiController
    {
        /// <summary>
        /// Logger for this class
        /// </summary>
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ReviewsController));

        /// <summary>
        /// GET Method for reviews belonging to a single session
        /// If no session id is provided the last finished session is used instead
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <param name="sessionId">Id of the session (default: null)</param>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns><see cref="ReviewsDTO"/> containing summary for the session reviews and penalties; <see cref="HttpError"/> on Error</returns>
        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult Get([FromUri] string leagueName, [FromUri] string sessionId = "0", [FromUri] string fields = null, bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get Reviews for session id: {sessionId} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }

                // parse sessionId
                long sessionIdValue;
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionIdValue = 0;
                }
                else if (long.TryParse(sessionId, out sessionIdValue) == false)
                {
                    return BadRequestInvalidType(nameof(sessionId), sessionId, sessionIdValue.GetType());
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                // Get reviews data from Data Access layer
                ReviewsDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    IReviewDataProvider reviewDataProvider = new ReviewDataProvider(dbContext);
                    data = reviewDataProvider.GetReviewsFromSession(sessionIdValue);
                }

                // return complete DTO or select fields
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
                logger.Error("Error in get Reviews", e);
                throw e;
            }
        }
    }
}