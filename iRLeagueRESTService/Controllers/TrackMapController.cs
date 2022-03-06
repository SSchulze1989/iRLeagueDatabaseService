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
    public class TrackMapController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TrackInfoController));

        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult Get(string id)
        {
            try
            {
                int trackId = int.Parse(id.Split('-').First());
                int configId = int.Parse(id.Split('-').Last());
                logger.Info($"Get Track maps request || trackId: {trackId} - configId: {configId}");

                var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Tracks_wMaps.xml");
                var locations = new LocationCollection(path);

                var mapsDict = locations
                    .Select(x => x.GetConfigInfo())
                    .GroupBy(x => (x.Track.TrackId, x.ConfigId), x => x.TrackMap)
                    .ToDictionary(x => x.Key, x => x.Select(y => y));
                IEnumerable<TrackMapSvg> maps;
                if (trackId != 0)
                {
                    maps = mapsDict[(trackId, configId)];
                }
                else
                {
                    maps = mapsDict.SelectMany(x => x.Value);
                }

                logger.Info($"Get Track maps request || send data: [{maps.Count()}] Tracks");
                return Ok(maps);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetTrackMaps", e);
                throw;
            }
        }

        [HttpGet]
        [ActionName("Get")]
        public IHttpActionResult GetTracks([FromUri]string[] ids = null)
        {
            try
            {
                logger.Info($"Get Track maps request || ids: [{string.Join(",", ids)}]");

                var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Tracks_wMaps.xml");
                var locations = new LocationCollection(path);

                var mapsDict = locations
                    .Select(x => x.GetConfigInfo())
                    .GroupBy(x => (x.Track.TrackId, x.ConfigId), x => x.TrackMap)
                    .ToDictionary(x => x.Key, x => x.Select(y => y));
                List<TrackMapSvg> maps = new List<TrackMapSvg>();
                if (ids == null)
                {
                    maps = mapsDict.SelectMany(x => x.Value).ToList();
                }
                else
                {
                    foreach(var idString in ids)
                    {
                        if (int.TryParse(idString.Split('-').First(), out int trackId) &&
                            int.TryParse(idString.Split('-').First(), out int configId))
                        {
                            maps.AddRange(mapsDict[(trackId, configId)]);
                        }
                    }
                }

                logger.Info($"Get Track mapss request || send data: [{maps.Count()}] Tracks");
                return Ok(maps);
            }
            catch (Exception e)
            {
                logger.Error("Error in GetTrackMaps", e);
                throw;
            }
        }
    }
}