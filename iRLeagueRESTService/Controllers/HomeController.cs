using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.Entities;
using LeagueDBService;

namespace iRLeagueRESTService.Controllers
{
    public class HomeController : ApiController
    {
        [HttpPost]
        public bool AddSeason()
        {
            return true;
        }

        //[HttpGet]
        //public SeasonDataDTO GetSeason(long id)
        //{
        //    ILeagueDBService dbService = new LeagueDBService.LeagueDBService();

        //    return dbService.GetSeason(id);
        //}
    }
}
