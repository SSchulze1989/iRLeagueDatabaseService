using iRLeagueRESTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Http;

namespace iRLeagueRESTService.Controllers
{
    /// <summary>
    /// A simple endpoint with a single get method that returns the DateTime of the last applied change to the referred league
    /// </summary>
    public class CheckUpdateController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get([FromUri] string leagueName)
        {
            var register = LeagueRegister.Get();
            if (register.Leagues.Any(x => x.Name == leagueName))
            {
                var leagueEntry = register.Leagues.Single(x => x.Name == leagueName);
                return Ok(leagueEntry.LastUpdate);
            }
            return BadRequest($"No register entry for {leagueName} found.");
        }
    }
}