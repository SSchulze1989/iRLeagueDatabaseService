using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Reviews;
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
    public class ReviewsController : LeagueApiController
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(ReviewsController));

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

                ReviewsDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    IReviewDataProvider reviewDataProvider = new ReviewDataProvider(dbContext);
                    data = reviewDataProvider.GetReviewsFromSession(sessionIdValue);
                }

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