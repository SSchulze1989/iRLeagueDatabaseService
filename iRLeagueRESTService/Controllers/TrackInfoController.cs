using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using iRLeagueManager.Locations;
using iRLeagueRESTService.Filters;
using log4net;

namespace iRLeagueRESTService.Controllers
{
    [IdentityBasicAuthentication]
    public class TrackInfoController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TrackInfoController));

        [HttpGet]
        [ActionName("Get")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult Get(int id)
        {
            try
            {
                logger.Info($"Get Tracks request || id: {id}");

                var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Tracks.xml");
                var locations = new LocationCollection(path);

                var tracks = locations.GetTrackList();
                if (id != 0)
                {
                    tracks = tracks.Where(x => x.TrackId == id);
                }

                logger.Info($"Get Tracks request || send data: [{tracks.Count()}] Tracks");
                return Ok(tracks);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetTracks", e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("Get")]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetTracks([FromUri]int[] ids)
        {
            try
            {
                logger.Info($"Get Tracks request || ids: [{string.Join(",", ids)}]");

                var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Tracks.xml");
                var locations = new LocationCollection(path);

                var tracks = locations.GetTrackList();
                if (ids != null && ids.Count() > 0)
                {
                    tracks = tracks.Where(x => ids.Contains(x.TrackId));
                }

                logger.Info($"Get Tracks request || send data: [{tracks.Count()}] Tracks");
                return Ok(tracks);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetTracks", e);
                throw;
            }
        }
    }
}