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
    /// <summary>
    /// Base class for League API endpoints
    /// Provides some basic functions needed by most controllers
    /// </summary>
    [IdentityBasicAuthentication]
    public class LeagueApiController : ApiController
    {
        /// <summary>
        /// Check if user has the required role for this action. 
        /// <para>Throws <see cref="HttpResponseException"/> with <see cref="HttpStatusCode.Unauthorized"/> if not in required role</para> 
        /// </summary>
        /// <param name="principal">Principal of the user</param>
        /// <param name="leagueName">Shortname of the league</param>
        /// <exception cref="HttpResponseException"></exception>
        protected void CheckLeagueRole(IPrincipal principal, string leagueName)
        {
            if (principal.IsInRole($"{leagueName}_User") || principal.IsInRole("Administrator"))
                return;

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Creates a BadRequest result with a message suitable for empty parameters
        /// </summary>
        /// <param name="parameter">Name of the parameter</param>
        /// <returns><see cref="BadRequestErrorMessageResult"/> with message for empty parameter</returns>
        protected internal virtual BadRequestErrorMessageResult BadRequestEmptyParameter(string parameter)
        {
            return BadRequest($"Empty Parameter!\nParameter {parameter} cannot be empty.");
        }

        /// <summary>
        /// Creates a BadRequest result with a message suitable for an invalid parameter type
        /// </summary>
        /// <param name="parameter">Name of the parameter</param>
        /// <param name="value">Provided parameter value</param>
        /// <param name="type">Required parameter type</param>
        /// <returns><see cref="BadRequestErrorMessageResult"/> with message for invalid parameter type</returns>
        protected internal virtual BadRequestErrorMessageResult BadRequestInvalidType(string parameter, string value, Type type)
        {
            return BadRequest($"Invalid parameter value!\nParameter {parameter} = \"{value}\" cannot be converted into {type.Name}.");
        }

        /// <summary>
        /// Create a new data context for use with the database access layer
        /// </summary>
        /// <param name="datbaseName">Full name of the target database</param>
        /// <returns><see cref="LeagueDbContext"/> of the target database</returns>
        protected virtual LeagueDbContext CreateDbContext(string datbaseName)
        {
            return new LeagueDbContext(datbaseName);
        }

        /// <summary>
        /// Get the full name of a league database provided its short name
        /// </summary>
        /// <param name="leagueName">Short name of the leage</param>
        /// <returns>Full name of the league database</returns>
        protected virtual string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return $"{leagueName}_leagueDb";
        }
    }
}