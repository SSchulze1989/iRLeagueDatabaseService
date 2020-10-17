//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Web;
//using System.Web.Http;
//using System.Data.Entity;
//using System.Runtime.Serialization;

//using iRLeagueDatabase;
//using iRLeagueDatabase.Mapper;
//using iRLeagueDatabase.DataTransfer;
//using iRLeagueDatabase.DataTransfer.Members;
//using iRLeagueDatabase.DataTransfer.Results;
//using iRLeagueDatabase.DataTransfer.Reviews;
//using iRLeagueDatabase.DataTransfer.Sessions;
//using iRLeagueDatabase.Entities;
//using iRLeagueDatabase.Entities.Members;
//using iRLeagueDatabase.Entities.Results;
//using iRLeagueDatabase.Entities.Reviews;
//using iRLeagueDatabase.Entities.Sessions;
//using iRLeagueRESTService.Filters;

//namespace iRLeagueRESTService.Controllers
//{
//    [KnownType(typeof(RaceSessionDataDTO))]
//    [KnownType(typeof(SessionDataDTO))]
//    [IdentityBasicAuthentication]
//    [Authorize]
//    public class HomeController : ApiController
//    {
//        private string TestUserName => System.Environment.GetEnvironmentVariable("IRLEAGUE_TESTUSER_NAME");
//        private string TestUserPassword => System.Environment.GetEnvironmentVariable("IRLEAGUE_TESTUSER_PASSWORD");

//        public string Get()
//        {
//            return "Erfolg!";
//        }

//        public MappableDTO Get(int requestId, string requestType)
//        {
//            ILeagueDBService dbService = new LeagueDBService.LeagueDBService();

//            return (dbService.DatabaseGET(new GETItemsRequestMessage()
//            {
//                userName = TestUserName,
//                password = TestUserPassword,
//                databaseName = "TestDatabase",
//                requestItemIds = new long[][] { new long[] { requestId } },
//                requestItemType = requestType,
//                requestResponse = true
//            }).items.FirstOrDefault() as MappableDTO);
//        }
        
//        [HttpGet]
//        public IHttpActionResult GetEntry([FromUri] string[] requestIds, [FromUri] string requestType, [FromUri] string databaseName)
//        {
//            long[][] requestIdValues = null;
            
//            if (requestIds.Count() > 0)
//                requestIdValues = requestIds.Select(x => x.Split(',').Select(y => long.Parse(y)).ToArray()).ToArray();

//            using (var dbService = new LeagueDBService.LeagueDBService())
//            {
//                var request = new GETItemsRequestMessage
//                {
//                    databaseName = databaseName,
//                    password = TestUserPassword,
//                    userName = TestUserName,
//                    requestItemIds = requestIdValues,
//                    requestItemType = requestType,
//                    requestResponse = true
//                };
//                if (request is GETItemsRequestMessage getRequest)
//                {
//                    var dbResult = dbService.DatabaseGET(getRequest);
//                    return (Ok(dbResult.items));
//                }
//            }

//            return BadRequest();
//        }

//        [HttpPut]
//        public IHttpActionResult PutEntry([FromBody] MappableDTO[] items, [FromUri] string requestType, [FromUri] string databaseName)
//        {
//            using (var dbService = new LeagueDBService.LeagueDBService())
//            {
//                var request = new PUTItemsRequestMessage
//                {
//                    databaseName = databaseName,
//                    password = TestUserPassword,
//                    userName = TestUserName,
//                    requestItemType = requestType,
//                    requestResponse = true,
//                    items = items
//                };
//                return (Ok(dbService.DatabasePUT(request)));
//            }
//        }

//        [HttpPost]
//        public IHttpActionResult PostEntry([FromBody] MappableDTO[] items, [FromUri] string requestType, [FromUri] string databaseName)
//        {
//            using (var dbService = new LeagueDBService.LeagueDBService())
//            {
//                var request = new POSTItemsRequestMessage
//                {
//                    databaseName = databaseName,
//                    password = TestUserPassword,
//                    userName = TestUserName,
//                    requestItemType = requestType,
//                    requestResponse = true,
//                    items = items
//                };
//                return (Ok(dbService.DatabasePOST(request).items));
//            }
//        }

//        [HttpDelete]
//        public IHttpActionResult DeleteEntry([FromUri] string[] requestIds, [FromUri] string requestType, [FromUri] string databaseName)
//        {
//            long[][] requestIdValues = null;

//            if (requestIds.Count() > 0)
//                requestIdValues = requestIds.Select(x => x.Split(',').Select(y => long.Parse(y)).ToArray()).ToArray();

//            using (var dbService = new LeagueDBService.LeagueDBService())
//            {
//                var request = new DELItemsRequestMessage
//                {
//                    databaseName = databaseName,
//                    password = TestUserName,
//                    userName = TestUserPassword,
//                    requestItemIds = requestIdValues,
//                    requestItemType = requestType,
//                    requestResponse = true
//                };
//                return (Ok(dbService.DatabaseDEL(request).success));
//            }
//        }

//        //[HttpPost]
//        //public IHttpActionResult DeleteEntry([FromBody] DELItemsRequestMessage request)
//        //{
//        //    using (var dbService = new LeagueDBService.LeagueDBService())
//        //    {
//        //        return (Ok(dbService.DatabaseDEL(request)));
//        //    }
//        //}

//        private MappableDTO[] GetFromDatabase(string databaseName, Type rqType, long[][] requestItemIds)
//        {
//            MappableDTO[] items = null;
//            object[][] requestIds = requestItemIds?.Select(x => x.Cast<object>().ToArray()).ToArray();

//            using (var dbContext = new LeagueDbContext(databaseName))
//            {
//                var mapper = new DTOMapper();
//                //responseMsg.databaseName = databaseName;

//                if (rqType.Equals(typeof(ScoredResultDataDTO)))
//                {
//                    //items = requestItemIds.Select(x => GetScoredResult(x[0], x[1])).ToArray();
//                }
//                else if (rqType.Equals(typeof(StandingsDataDTO)))
//                {
//                    dbContext.Configuration.LazyLoadingEnabled = false;
//                    List<StandingsDataDTO> responseItems = new List<StandingsDataDTO>();
//                    foreach (var itemIdArray in requestIds)
//                    {
//                        var itemId = (long)itemIdArray[0];
//                        //var scoring = dbContext.Set<ScoringEntity>()
//                        //    .Include(x => x.Sessions)
//                        //    .Include(x => x.MultiScoringResults.Select(y => y.Sessions))
//                        //    .SingleOrDefault(x => x.ScoringId == itemId);
//                        //var scoredResults = dbContext.Set<ScoredResultEntity>().Where(x => x.ScoringId == scoring.ScoringId || scoring.MultiScoringResults.Any(y => y.ScoringId == x.ScoringId));
//                        //var scoredResultRows = dbContext.Set<ScoredResultRowEntity>().Where(x => scoredResults.Any(y => x.ScoredResultId == y.ResultId && y.ScoringId == y.ScoringId));
//                        //var results = dbContext.Set<ResultEntity>().Where(x => scoredResults.Any(y => y.ResultId == x.ResultId));
//                        //var resultRows = dbContext.Set<ResultRowEntity>().Where(x => results.Any(y => y.ResultId == x.ResultId));
//                        ////var sessions = dbContext.Set<SessionBaseEntity>().Where(x => results.Any(y => y.ResultId == x.SessionId));
//                        //dbContext.Set<LeagueMemberEntity>().Where(x => resultRows.Any(y => y.MemberId == x.MemberId));
//                        var scoring = dbContext.Set<ScoringEntity>()
//                            .Include(x => x.Sessions.Select(y => y.SessionResult))
//                            .Include(x => x.ScoredResults.Select(y => y.FinalResults.Select(z => z.ResultRow.Member)))
//                            .Include(x => x.MultiScoringResults.Select(y => y.Sessions.Select(z => z.SessionResult)))
//                            //.Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Result.Session.Schedule.Season))))
//                            .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Member))))
//                            .FirstOrDefault(x => x.ScoringId == itemId);


//                        if (scoring != null)
//                        {

//                            var standings = scoring.GetSeasonStandings();
//                            responseItems.Add(mapper.MapTo<StandingsDataDTO>(standings));
//                        }
//                    }
//                    items = responseItems.ToArray();
//                }
//                else
//                {
//                    var rqEntityType = mapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(rqType))?.SourceType;
//                    if (rqEntityType == null)
//                        return null;

//                    List<MappableDTO> resultItems = new List<MappableDTO>();

//                    List<object> entities = new List<object>();

//                    if (requestIds != null)
//                    {
//                        foreach (var keys in requestIds)
//                        {
//                            object entity = dbContext.Set(rqEntityType).Find(keys);
//                            if (entity == null)
//                                throw new Exception("Entity not found in Database - Type: " + rqEntityType.Name + " || keys: { " + keys.Select(x => x.ToString()).Aggregate((x, y) => ", ") + " }");

//                            entities.Add(entity);
//                        }
//                    }
//                    else
//                    {
//                        entities = dbContext.Set(rqEntityType).ToListAsync().Result;
//                    }

//                    foreach (var entity in entities)
//                    {
//                        var dto = mapper.MapTo(entity, rqType) as MappableDTO;
//                        resultItems.Add(dto);
//                    }
//                    items = resultItems.ToArray();
//                }
//            };

//            //responseMsg.status = "success";

//            //GC.Collect();
//            return items;
//        }
//    }
//}
