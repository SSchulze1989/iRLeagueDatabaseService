using iRLeagueDatabase.DataTransfer.Convenience;
using iRLeagueDatabase.Extensions;
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
    /// <summary>
    /// Get convenience data for a single season or all seasons on the league
    /// </summary>
    public class SeasonsController : LeagueApiController
    {
        /// <summary>
        /// Logger for this class
        /// </summary>
        private static readonly ILog logger = log4net.LogManager.GetLogger(typeof(SeasonsController));

        /// <summary>
        /// GET data for a single season
        /// If seasonId == 0 the current season is returned
        /// </summary>
        /// <param name="leagueName">Short name of the league</param>
        /// <paramref name="seasonId">Id of the requested season</paramref>
        /// <param name="fields">Comma separated string to specify exact fields to return - JSON only! (default: null)</param>
        /// <param name="excludeFields">If True, specified <paramref name="fields"/> will be excluded - JSON only! (default: false)</param>
        /// <returns><see cref="SeasonConvenienceDTO"/> of the season</returns>
        [HttpGet]
        [LeagueAuthorize(Roles = iRLeagueDatabase.Enums.LeagueRoleEnum.User)]
        public IHttpActionResult Get([FromUri] string leagueName, [FromUri] long seasonId, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get data for season id: {seasonId} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                // Get session data from data access layer
                SeasonConvenieneDTO data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    ISeasonDataProvider sessionsDataProvider = new SeasonDataProvider(dbContext);
                    data = sessionsDataProvider.GetSeason(seasonId);
                }

                // return complete DTO or select fields
                logger.Info($"Send data - {nameof(SeasonConvenieneDTO)} id: {data?.SeasonId}");
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
                logger.Error("Error!", e);
                throw e;
            }
        }

        [HttpGet]
        [LeagueAuthorize(Roles = iRLeagueDatabase.Enums.LeagueRoleEnum.User)]
        public IHttpActionResult GetSeasons([FromUri] string leagueName, [FromUri] string seasonIds = null, [FromUri] string fields = null, [FromUri] bool excludeFields = false)
        {
            try
            {
                logger.Info($"Get data for season ids: {seasonIds} - league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                // check for empty parameters
                if (string.IsNullOrEmpty(leagueName))
                {
                    return BadRequestEmptyParameter(nameof(leagueName));
                }
                // try to parse seasonIds string
                List<long> seasonIdValues = null;
                if (seasonIds != null)
                {
                    seasonIdValues = new List<long>();
                    foreach (var idString in seasonIds.Split(','))
                    {
                        if (long.TryParse(idString, out long id) == false)
                        {
                            return BadRequestInvalidType(nameof(seasonIds), idString, typeof(long));
                        }
                        seasonIdValues.Add(id);
                    }
                }

                var databaseName = GetDatabaseNameFromLeagueName(leagueName);

                // Get session data from data access layer
                SeasonConvenieneDTO[] data;
                using (var dbContext = CreateDbContext(databaseName))
                {
                    ISeasonDataProvider sessionsDataProvider = new SeasonDataProvider(dbContext);
                    data = sessionsDataProvider.GetSeasons(seasonIdValues?.ToArray());
                }

                // return complete DTO or select fields
                logger.Info($"Send data - {nameof(SeasonConvenieneDTO)}[{data.Count()}] ids: {string.Join(",", data.Select(x => x?.SeasonId))}");
                if (string.IsNullOrEmpty(fields))
                {
                    return Ok(data);
                }
                else
                {
                    data.ForEach(x => x.SetSerializableProperties(fields.Split(','), excludeFields));
                    var response = data.Select(x => SelectFieldsHelper.GetSelectedFieldObject(x));
                    return Json(response);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error!", e);
                throw e;
            }
        }
    }
}