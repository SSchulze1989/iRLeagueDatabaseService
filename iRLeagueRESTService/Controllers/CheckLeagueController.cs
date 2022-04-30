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
using iRLeagueRESTService.Models;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class CheckLeagueController : LeagueApiController
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
                base.CheckLeagueRole(User, leagueName);
                
                var leagueInfo = GetLeagueRegisterInfo(id);
                return Ok(leagueInfo);
            }
            else
                return BadRequest($"League {id} does not exist");
        }

        [HttpPost]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult PostLeague([FromUri] string leagueName, [FromUri] string fullName = "")
        {
            // check for empty body and invalid data
            if (string.IsNullOrEmpty(leagueName))
            {
                return BadRequestEmptyParameter(nameof(leagueName));
            }
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = leagueName;
            }
            base.CheckLeagueRole(User, leagueName);

            var dbName = GetDatabaseNameFromLeagueName(leagueName);
            var register = LeagueRegister.Get();

            var league = register.GetLeague(leagueName);

            if (league == null)
            {
                // check current number of leagues for that user
                string userId = User.Identity.GetUserId();
                var leagueCount = register.Leagues.Count(x => x.CreatorId.ToString() == userId);
                //var maxLeagues = 3;
                //if (leagueCount >= maxLeagues)
                //{
                //    return BadRequest($"Create league failed. Maximum numbers of {maxLeagues} leagues per user reached.");
                //}

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
                        // Create new entry in league register
                        return Ok($"New League {leagueName} with database {dbName} created!");
                    }
                    UpdateLeague(leagueName, User);
                    return BadRequest($"League {leagueName} already exists!");
                }
            }

            // if league exists just update fullname
            league.PrettyName = fullName;
            register.Save();
            return Ok("League information updated");
        }

        //public static string GetDatabaseNameFromLeagueName(string leagueName)
        //{
        //    return $"{leagueName}_leagueDb";
        //}

        //public static string GetLeagueNameFromDatabaseName(string dbName)
        //{
        //    return dbName.Substring(0, dbName.Length - "_leagueDb".Length);
        //}

        private new bool CheckLeagueRole(IPrincipal principal, string leagueName)
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
            catch
            {
                result = false;
            }

            return result;
        }
    }
}