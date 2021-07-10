using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.Enums;
using iRLeagueDatabase.Extensions;
using iRLeagueRESTService.Filters;
using iRLeagueRESTService.Models;
using Microsoft.AspNet.Identity;
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
        protected virtual LeagueDbContext CreateDbContext(string leagueName)
        {
            return new LeagueDbContext(leagueName);
        }

        /// <summary>
        /// Get the full name of a league database provided its short name
        /// </summary>
        /// <param name="leagueName">Short name of the leage</param>
        /// <returns>Full name of the league database</returns>
        protected virtual string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return leagueName;
        }

        /// <summary>
        /// Get the league name from the full database name
        /// </summary>
        /// <param name="dbName">full database name</param>
        /// <returns>Shortname of the league</returns>
        public static string GetLeagueNameFromDatabaseName(string dbName)
        {
            return dbName;
        }

        /// <summary>
        /// Set the update status of the current league
        /// </summary>
        /// <param name="leagueName">Shortname of the league</param>
        /// <param name="principal">Principal of the current User</param>
        protected void UpdateLeague(string leagueName, IPrincipal principal)
        {
            var register = LeagueRegister.Get();

            LeagueEntry leagueEntry = null;
            if (register.Leagues.Any(x => x.Name == leagueName) == false)
            {
                leagueEntry = new LeagueEntry()
                {
                    Name = leagueName,
                    CreatorName = principal.Identity.Name,
                    OwnerId = Guid.Parse(principal.Identity.GetUserId()),
                    CreatorId = Guid.Parse(principal.Identity.GetUserId()),
                    CreatedOn = DateTime.Now,
                    PrettyName = leagueName
                };
                register.Leagues.Add(leagueEntry);
            }
            else
            {
                leagueEntry = register.Leagues.SingleOrDefault(x => x.Name == leagueName);
            }
            leagueEntry.LastUpdate = DateTime.Now;
            if (leagueEntry.OwnerId == null)
            {
                leagueEntry.OwnerId = leagueEntry.CreatorId;
            }

            register.Save();
        }

        /// <summary>
        /// Get informaiton about the league from the league register
        /// </summary>
        /// <param name="leagueName">Shortname of the league</param>
        /// <returns>DTO containing league information</returns>
        protected LeagueDTO GetLeagueRegisterInfo(string leagueName)
        {
            var register = LeagueRegister.Get();

            var leagueEntry = register.GetLeague(leagueName);
            if (leagueEntry == null)
            {
                return null;
            }

            var leagueDto = new LeagueDTO()
            {
                Name = leagueEntry.Name,
                LongName = leagueEntry.PrettyName,
                CreatedByUserId = leagueEntry.CreatorId.ToString(),
                CreatedByUserName = leagueEntry.CreatorName,
                CreatedOn = leagueEntry.CreatedOn,
                LastModifiedOn = leagueEntry.LastUpdate,
                OwnerUserId = leagueEntry.OwnerId.ToString()
            };

            return leagueDto;
        }

        /// <summary>
        /// Get the league role flags from the provided IPrincipal
        /// </summary>
        /// <param name="user">User that has roles</param>
        /// <param name="leagueName">Name of the league</param>
        /// <returns></returns>
        protected LeagueRoleEnum GetUserLeagueRoles(IPrincipal user, string leagueName)
        {
            if (user == null || user.Identity.IsAuthenticated == false)
            {
                return LeagueRoleEnum.None;
            }

            if (user.IsInRole("Administrator"))
            {
                return LeagueRoleEnum.Admin;
            }

            var availableRoles = LeagueRoles.GetAvailableRoles();
            LeagueRoleEnum userRoles = 0;

            foreach (var role in availableRoles)
            {
                var leagueRoleName = $"{leagueName}_{role.Value}";
                if (user.IsInRole(leagueRoleName))
                {
                    userRoles |= role.Key;
                }
            }

            return userRoles;
        }
    }
}