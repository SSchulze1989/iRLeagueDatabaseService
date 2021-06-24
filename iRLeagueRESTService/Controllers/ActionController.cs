using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.Runtime.Serialization;

using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataAccess.Provider;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using System.Security.Principal;
using System.Net;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class ActionController : ApiController
    {
        //[HttpPut]
        //[ActionName("CalcResult")]
        //[Authorize(Roles = LeagueRoles.UserOrAdmin)]
        //public IHttpActionResult CalculateResultTrigger([FromUri] string sessionId, string leagueName)
        //{
        //    if (sessionId == null || leagueName == null)
        //    {
        //        return BadRequest("Parameters can not be null");
        //    }

        //    if (long.TryParse(sessionId, out long sessionIdValue) == false)
        //    {
        //        return BadRequest("Invalid sessionId format. SessionId must be numeric");
        //    }

        //    using (ILeagueActionProvider leagueActionProvider = new LeagueActionProvider(new LeagueDbContext(leagueName)))
        //    {
        //        leagueActionProvider.CalculateScoredResult(sessionIdValue);
        //    }

        //    return Ok();
        //}

        [HttpPut]
        [ActionName("CalcResultArray")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult CalculateResultsTrigger([FromUri] string[] requestIds, string leagueName)
        {
            CheckLeagueRole(User, leagueName);

            if (requestIds == null || leagueName == null)
            {
                return BadRequest("Parameters can not be null");
            }

            List<long> sessionIdValues = new List<long>();
            foreach(var sessionId in requestIds)
            {
                if (long.TryParse(sessionId, out long sessionIdValue) == false)
                {
                    return BadRequest("Invalid sessionId format. SessionId must be numeric");
                }
                sessionIdValues.Add(sessionIdValue);
            }

            using (ILeagueActionProvider leagueActionProvider = new LeagueActionProvider(new LeagueDbContext(GetDatabaseNameFromLeagueName(leagueName))))
            {
                leagueActionProvider.CalculateScoredResultArray(sessionIdValues.ToArray());
            }

            return Ok();
        }

        private string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return $"{leagueName}_leagueDb";
        }

        private void CheckLeagueRole(IPrincipal principal, string leagueName)
        {
            if (principal.IsInRole($"{leagueName}_User") || principal.IsInRole("Administrator"))
                return;

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }
    }
}