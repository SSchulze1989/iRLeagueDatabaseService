using iRLeagueDatabase;
using iRLeagueRESTService.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class LeagueApiController : ApiController
    {
        protected void CheckLeagueRole(IPrincipal principal, string leagueName)
        {
            if (principal.IsInRole($"{leagueName}_User") || principal.IsInRole("Administrator"))
                return;

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        protected internal virtual BadRequestErrorMessageResult BadRequestEmptyParameter(string parameter)
        {
            return BadRequest($"Empty Parameter!\nParameter {parameter} cannot be empty.");
        }

        protected internal virtual BadRequestErrorMessageResult BadRequestInvalidType(string parameter, string value, Type type)
        {
            return BadRequest($"Invalid parameter value!\nParameter {parameter} = \"{value}\" cannot be converted into {type.Name}.");
        }

        protected virtual LeagueDbContext CreateDbContext(string datbaseName)
        {
            return new LeagueDbContext(datbaseName);
        }

        protected virtual string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return $"{leagueName}_leagueDb";
        }
    }
}