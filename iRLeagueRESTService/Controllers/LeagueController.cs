using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using System.Runtime.Serialization;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using System.Security.Principal;
using Microsoft.AspNet.Identity.EntityFramework;

using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using System.Net;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class CheckLeagueController : ApiController
    {
        [HttpGet]
        [ActionName("Get")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetLeagueInfo([FromUri] string leagueName)
        {
            var databaseExists = CheckDatabaseExists(new SqlConnection("server=(local)\\IRLEAGUEDB;Trusted_Connection=yes"), GetDatabaseNameFromLeagueName(leagueName));

            if (databaseExists)
            {
                if (CheckLeagueRole(User, leagueName))
                    return Ok();
                else
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            else
                return BadRequest("Database does not exist");
        }

        public static string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return leagueName;
        }

        private bool CheckLeagueRole(IPrincipal principal, string leagueName)
        {
            if (principal.IsInRole($"{leagueName}_User") || principal.IsInRole("Administrator"))
                return true;
            return false;
        }

        private static bool CheckDatabaseExists(SqlConnection tmpConn, string databaseName)
        {
            string sqlCreateDBQuery;
            bool result = false;

            try
            {
                tmpConn = new SqlConnection("server=(local)\\IRLEAGUEDB;Trusted_Connection=yes");

                sqlCreateDBQuery = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);
        
        using (tmpConn)
                {
                    using (SqlCommand sqlCmd = new SqlCommand(sqlCreateDBQuery, tmpConn))
                    {
                        tmpConn.Open();

                        object resultObj = sqlCmd.ExecuteScalar();

                        int databaseID = 0;

                        if (resultObj != null)
                        {
                            int.TryParse(resultObj.ToString(), out databaseID);
                        }

                        tmpConn.Close();

                        result = (databaseID > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}