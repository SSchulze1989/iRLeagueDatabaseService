using iRLeagueDatabase.DataTransfer.Statistics.Convenience;
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
    [IdentityBasicAuthentication]
    public class StatsController : LeagueApiController
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(StandingsController));

        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult Get([FromUri] string leagueName, [FromUri] long seasonId = 0, [FromUri] bool seasonStats = true, [FromUri] bool leagueStats = true, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get statistics for league: {leagueName} - season: {seasonId}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                StatisticConvenienceDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    IStatsDataProvider statsDataProvider = new StatsDataProvider(dbContext);
                    data = statsDataProvider.GetStatistics(seasonId, seasonStats, leagueStats);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - StatisticSets for seasonId: {data.SeasonStats?.FirstOrDefault()?.SeasonId}");
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
                logger.Error("Error in get Statistics", e);
                throw e;
            }
        }
    }
}