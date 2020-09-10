using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Mapper;

namespace iRLeagueRESTService.Data
{
    public class ModelDataProvider<TModelDTO> : IModelDataProvider<TModelDTO, long[]>, IDisposable where TModelDTO : class, IMappableDTO
    {
        private LeagueDbContext DbContext { get; }

        public string UserName { get; set; }
        public string UserId { get; set; }

        public ModelDataProvider()
        {
            DbContext = new LeagueDbContext();
        }

        public ModelDataProvider(LeagueDbContext context)
        {
            DbContext = context;
        }

        public ModelDataProvider(LeagueDbContext context, string userName) : this(context)
        {
            UserName = userName;
        }

        public ModelDataProvider(LeagueDbContext context, string userName, string userId)  : this(context, userName)
        {
            UserId = userId;
        }

        public void Dispose()
        {
            ((IDisposable)DbContext).Dispose();
        }

        public TModelDTO Get(Type requestType, long[] requestId)
        {
            long[][] requestIds;
            if (requestId != null)
                requestIds = new long[][] { requestId };
            else
                requestIds = null;

            var data = GetArray(requestType, requestIds);
            if (data == null || data.Count() == 0)
                return null;

            return data.First();
        }

        public TModelDTO[] GetArray(Type requestType, long[][] requestIds)
        {
            TModelDTO[] items = null;

            var mapper = new DTOMapper();

            if (requestType.Equals(typeof(ScoredResultDataDTO)))
            {
                //var leagueService = new LeagueDBService.LeagueDBService();
                items = requestIds.Select(x => GetScoredResult(x[0], x[1])).Cast<TModelDTO>().ToArray();
            }
            else if (requestType.Equals(typeof(StandingsDataDTO)))
            {
                //var scoringIds = requestIds.Select(x => x[0]).ToArray();
                //var sessionIds = requestIds.Select(x => x.Count() > 0 ? x[1] : 0).ToArray();
                items = GetStandings(requestIds).Cast<TModelDTO>().ToArray();
            }
            else
            {
                var rqEntityType = mapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(requestType))?.SourceType;
                if (rqEntityType == null)
                    return null;

                List<TModelDTO> resultItems = new List<TModelDTO>();

                List<object> entities = new List<object>();

                if (requestIds != null)
                {
                    var requestKeys = requestIds.Select(x => x.Cast<object>().ToArray()).ToArray();
                    foreach (var keys in requestKeys)
                    {
                        object entity = DbContext.Set(rqEntityType).Find(keys);
                        //if (entity == null)
                            //throw new Exception("Entity not found in Database - Type: " + rqEntityType.Name + " || keys: { " + keys.Select(x => x.ToString()).Aggregate((x, y) => ", ") + " }");
                        if (entity != null)
                            entities.Add(entity);
                    }
                }
                else
                {
                    entities = DbContext.Set(rqEntityType).ToListAsync().Result;
                }

                foreach (var entity in entities)
                {
                    var dto = mapper.MapTo(entity, requestType) as TModelDTO;
                    resultItems.Add(dto);
                }
                items = resultItems.ToArray();
            }

            return items;
        }

        public TModelDTO Post(Type requestType, TModelDTO data)
        {
            if (data == null)
                return null;

            var dataArray = PostArray(requestType, new TModelDTO[] { data });
            if (dataArray == null || dataArray.Count() == 0)
                return null;

            return dataArray.First();
        }

        public TModelDTO[] PostArray(Type requestType, TModelDTO[] data)
        {
            TModelDTO[] items = data;

            if (items == null)
                return null;

            var searchNames = new string[]
            {
                "iRLeagueDatabase.DataTransfer.",
                "iRLeagueDatabase.DataTransfer.Members.",
                "iRLeagueDatabase.DataTransfer.Results.",
                "iRLeagueDatabase.DataTransfer.Reviews.",
                "iRLeagueDatabase.DataTransfer.Sessions."
            };

            var entityMapper = new EntityMapper(DbContext) { UserName = UserName, UserId = UserId };
            var dtoMapper = new DTOMapper();

            var rqEntityType = dtoMapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(requestType))?.SourceType;
            if (rqEntityType == null)
                return null;

            var dbSet = DbContext.Set(rqEntityType);

            List<TModelDTO> resultItems = new List<TModelDTO>();
            foreach (var item in items)
            {
                object entity = dbSet.Find(item.Keys);
                if (entity == null)
                {
                    entity = dbSet.Create();
                    dbSet.Add(entity);
                }
                entityMapper.MapTo(item, entity, requestType, rqEntityType);

                DbContext.SaveChanges();

                var dto = dtoMapper.MapTo(entity, requestType) as TModelDTO;
                resultItems.Add(dto);
            }
            items = resultItems.ToArray();

            return items;
        }

        public TModelDTO Put(Type requestType, TModelDTO data)
        {
            if (data == null)
                return null;

            var dataArray = PutArray(requestType, new TModelDTO[] { data });
            if (dataArray == null || dataArray.Count() == 0)
                return null;

            return dataArray.First();
        }

        public TModelDTO[] PutArray(Type requestType, TModelDTO[] data)
        {
            TModelDTO[] items = data;
            
            var entityMapper = new EntityMapper(DbContext) { UserName = UserName, UserId = UserId };
            var dtoMapper = new DTOMapper();

            var rqEntityType = dtoMapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(requestType))?.SourceType;
            if (rqEntityType == null)
                return null;

            List<TModelDTO> resultItems = new List<TModelDTO>();
            foreach (object item in items)
            {
                object entity = entityMapper.MapTo(item, null, requestType, rqEntityType);
                try
                {
                    DbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    throw;
                }
                var dto = dtoMapper.MapTo(entity, requestType) as TModelDTO;
                resultItems.Add(dto);
            }
            items = resultItems.ToArray();

            return items;
        }

        public bool Delete(Type requestType, long[] requestId)
        {
            long[][] requestIds;
            if (requestId != null)
                requestIds = new long[][] { requestId };
            else
                return false;

            var data = DeleteArray(requestType, requestIds);

            return data;
        }

        public bool DeleteArray(Type requestType, long[][] requestIds)
        {
            bool status = false;

            var mapper = new DTOMapper();
            var rqEntityType = mapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(requestType))?.SourceType;
            if (rqEntityType == null)
                throw new Exception("No typemap for " + requestType.Name + " found");
            var requestKeys = requestIds.Select(x => x.Cast<object>().ToArray()).ToArray();
            foreach (var keys in requestKeys)
            {
                var entity = DbContext.Set(rqEntityType).Find(keys) as MappableEntity;

                if (entity != null)
                {
                    //dbContext.Set(rqEntityType).Remove(entity);
                    entity.Delete(DbContext);
                    status  = true;
                }
            }
            DbContext.SaveChanges();

            return status;
        }

        private ScoredResultDataDTO GetScoredResult(long sessionId, long scoringId)
        {
            var scoredResultData = new ScoredResultDataDTO();

            DbContext.Configuration.LazyLoadingEnabled = false;
            var scoredResultEntity = DbContext.Set<ScoredResultEntity>()
                //.AsNoTracking()
                //.Include(x => x.Result.Session)
                //.Include(x => x.Scoring)
                //.Include(x => x.Result.RawResults.Select(y => y.Member))
                //.Include(x => x.Result.RawResults.Select(y => y.ScoredResultRows))
                //.Include(x => x.FinalResults.Select(y => y.ResultRow.Member))
                .FirstOrDefault(x => x.ResultId == sessionId && x.ScoringId == scoringId);

            if (scoredResultEntity == null)
                return new ScoredResultDataDTO()
                {
                    ResultId = sessionId,
                    Scoring = new ScoringInfoDTO() { ScoringId = scoringId }
                };

            DbContext.Entry(scoredResultEntity).Reference(x => x.Scoring).Load();
            DbContext.Entry(scoredResultEntity).Reference(x => x.Result).Query().Include(x => x.Session).Load();
            DbContext.Entry(scoredResultEntity).Collection(x => x.FinalResults).Query()
                .Include(x => x.AddPenalty)
                .Include(x => x.ResultRow.Member.Team).Load();

            if (scoredResultEntity is ScoredTeamResultEntity scoredTeamResultEntity)
            {
                DbContext.Entry(scoredTeamResultEntity).Collection(x => x.TeamResults).Query()
                    .Include(x => x.ScoredResultRows).Load();
            } 

            var mapper = new DTOMapper();
            scoredResultData = mapper.MapTo<ScoredResultDataDTO>(scoredResultEntity);
            DbContext.Configuration.LazyLoadingEnabled = true;

            return scoredResultData;
        }

        private StandingsDataDTO[] GetStandings(long[] scoringIds)
        {
            return GetStandings(scoringIds.Select(x => new long[] { x }).ToArray());
        }

        private StandingsDataDTO[] GetStandings(long[][] requestIds)
        {
            var mapper = new DTOMapper();

            DbContext.Configuration.LazyLoadingEnabled = false;
            List<StandingsDataDTO> responseItems = new List<StandingsDataDTO>();
            foreach (var requestId in requestIds)
            {
                var scoringTableId = requestId[0];
                var sessionId = requestId.Count() > 1 ? requestId[1] : 0;

                var scoringTable = DbContext.Set<ScoringTableEntity>()
                    .Where(x => x.ScoringTableId == scoringTableId)
                    .Include(x => x.Scorings.Select(y => y.Sessions.Select(z => z.SessionResult)))
                    .Include(x => x.Scorings.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                    .FirstOrDefault();

                if (scoringTable != null)
                {
                    StandingsEntity standings;
                    if (sessionId == 0)
                        standings = scoringTable.GetSeasonStandings();
                    else
                        standings = scoringTable.GetSeasonStandings(scoringTable.GetAllSessions().SingleOrDefault(x => x.SessionId == sessionId));
                    var standingsDTO = mapper.MapTo<StandingsDataDTO>(standings);
                    standingsDTO.SessionId = sessionId;
                    responseItems.Add(standingsDTO);
                }

                //var scoringId = requestId[0];
                //var sessionId = requestId.Count() > 1 ? requestId[1] : 0;
                ////var scoring = dbContext.Set<ScoringEntity>()
                ////    .Include(x => x.Sessions)
                ////    .Include(x => x.MultiScoringResults.Select(y => y.Sessions))
                ////    .SingleOrDefault(x => x.ScoringId == itemId);
                ////var scoredResults = dbContext.Set<ScoredResultEntity>().Where(x => x.ScoringId == scoring.ScoringId || scoring.MultiScoringResults.Any(y => y.ScoringId == x.ScoringId));
                ////var scoredResultRows = dbContext.Set<ScoredResultRowEntity>().Where(x => scoredResults.Any(y => x.ScoredResultId == y.ResultId && y.ScoringId == y.ScoringId));
                ////var results = dbContext.Set<ResultEntity>().Where(x => scoredResults.Any(y => y.ResultId == x.ResultId));
                ////var resultRows = dbContext.Set<ResultRowEntity>().Where(x => results.Any(y => y.ResultId == x.ResultId));
                //////var sessions = dbContext.Set<SessionBaseEntity>().Where(x => results.Any(y => y.ResultId == x.SessionId));
                ////dbContext.Set<LeagueMemberEntity>().Where(x => resultRows.Any(y => y.MemberId == x.MemberId));
                //var scoring = DbContext.Set<ScoringEntity>()
                //    .Where(x => x.ScoringId == scoringId)
                //    .Include(x => x.Sessions.Select(y => y.SessionResult))
                //    .Include(x => x.ScoredResults.Select(y => y.FinalResults.Select(z => z.ResultRow.Member)))
                //    .Include(x => x.MultiScoringResults.Select(y => y.Sessions.Select(z => z.SessionResult)))
                //    //.Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Result.Session.Schedule.Season))))
                //    .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Member))))
                //    //.Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.AddPenalty))))
                //    .FirstOrDefault(x => x.ScoringId == scoringId);

                ////var test = ((System.Data.Entity.Infrastructure.DbQuery<ScoringEntity>)DbContext.Set<ScoringEntity>()
                ////    .Include(x => x.Sessions.Select(y => y.SessionResult))
                ////    .Include(x => x.ScoredResults.Select(y => y.FinalResults.Select(z => z.ResultRow.Member)))
                ////    .Include(x => x.MultiScoringResults.Select(y => y.Sessions.Select(z => z.SessionResult)))
                ////    //.Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Result.Session.Schedule.Season))))
                ////    .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Member))))
                ////    .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.AddPenalty))))).Sql;

                ////test = ((System.Data.Entity.Infrastructure.DbQuery<ScoringEntity>)DbContext.Set<ScoringEntity>()
                ////    .Where(x => x.ScoringId == scoringId)
                ////    .Include(x => x.Sessions.Select(y => y.SessionResult))
                ////    .Include(x => x.ScoredResults.Select(y => y.FinalResults.Select(z => z.ResultRow.Member)))
                ////    .Include(x => x.MultiScoringResults.Select(y => y.Sessions.Select(z => z.SessionResult)))
                ////    //.Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Result.Session.Schedule.Season))))
                ////    .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.ResultRow.Member))))
                ////    .Include(x => x.MultiScoringResults.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(n => n.AddPenalty))))).Sql;


                //if (scoring != null)
                //{
                //    StandingsEntity standings;
                //    if (sessionId == 0)
                //        standings = scoring.GetSeasonStandings();
                //    else
                //        standings = scoring.GetSeasonStandings(scoring.GetAllSessions().SingleOrDefault(x => x.SessionId == sessionId));
                //    var standingsDTO = mapper.MapTo<StandingsDataDTO>(standings);
                //    standingsDTO.SessionId = sessionId;
                //    responseItems.Add(standingsDTO);
                //}
            }
            DbContext.Configuration.LazyLoadingEnabled = true;

            return responseItems.ToArray();
        }
    }

    public class ModelDataProvider : ModelDataProvider<MappableDTO>, IModelDataProvider
    {
        public ModelDataProvider(LeagueDbContext context) : base(context) { }

        public ModelDataProvider(LeagueDbContext context, string userName) : base(context, userName) { }
        public ModelDataProvider(LeagueDbContext context, string userName, string userId) : base(context, userName, userId) { }
    }
}