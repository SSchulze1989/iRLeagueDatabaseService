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
using System.Data;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class CheckLeagueController : ApiController
    {    
        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult ReturnLeagueNames()
        {
            var leagueNames = GetDatabaseList()
                .Where(x => x.Contains("_leagueDb"))
                .Where(x => CheckLeagueRole(User, GetLeagueNameFromDatabaseName(x)))
                .Select(GetLeagueNameFromDatabaseName)
                .ToArray();
            return Ok(leagueNames);
        }

        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetLeagueInfo([FromUri] string id)
        {
            var leagueName = id;
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

        [HttpPost]
        [Authorize(Roles = LeagueRoles.Admin)]
        public IHttpActionResult CreateLeague([FromUri] string id)
        {
            if (id == null)
                return BadRequest("League name must not be empty!");

            var dbName = GetDatabaseNameFromLeagueName(id);

            using (var dbContext = new LeagueDbContext(dbName, createDb: true))
            {
                var seasons = dbContext.Seasons;
                if (seasons == null || seasons.Count() == 0)
                {
                    seasons.Add(new iRLeagueDatabase.Entities.SeasonEntity()
                    {
                        SeasonName = "First Season",
                        CreatedByUserName = User.Identity.Name,
                        CreatedByUserId = User.Identity.GetUserId(),
                        LastModifiedByUserName = User.Identity.Name,
                        LastModifiedByUserId = User.Identity.GetUserId()
                    });
                    dbContext.SaveChanges();
                    return Ok($"New League {id} with database {dbName} created!");
                }
                return BadRequest("League already exists");
            }
        }

        public static string GetDatabaseNameFromLeagueName(string leagueName)
        {
            return $"{leagueName}_leagueDb";
        }

        public static string GetLeagueNameFromDatabaseName(string dbName)
        {
            return dbName.Substring(0, dbName.Length - "_leagueDb".Length);
        }

        private bool CheckLeagueRole(IPrincipal principal, string leagueName)
        {
            if (principal.IsInRole($"{leagueName}_User") || principal.IsInRole("Administrator"))
                return true;
            return false;
        }

        private List<string> GetDatabaseList()
        {
            List<string> list = new List<string>();

            // Open connection to the database
            string conString = "server=(local)\\IRLEAGUEDB;Trusted_Connection=yes";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return list;
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