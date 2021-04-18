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
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace iRLeagueRESTService.Data
{
    public class ModelDataProvider<TModelDTO> : DataProviderBase, IModelDataProvider<TModelDTO, long[]>, IDisposable where TModelDTO : class, IMappableDTO
    {

        public ModelDataProvider() : base(new LeagueDbContext())
        {
        }

        public ModelDataProvider(LeagueDbContext context) : base (context)
        {
        }

        public ModelDataProvider(LeagueDbContext context, string userName) : base(context, userName)
        {
        }

        public ModelDataProvider(LeagueDbContext context, string userName, string userId) : base(context, userName, userId)
        {
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
            TModelDTO[] items = new TModelDTO[0];

            var mapper = new DTOMapper(DbContext);

            if (requestType.Equals(typeof(ScoredResultDataDTO)))
            {
                //var leagueService = new LeagueDBService.LeagueDBService();
                if (requestIds != null && requestIds.Count() > 0)
                {
                    items = requestIds.Select(x => GetScoredResult(x[0], x[1])).Cast<TModelDTO>().ToArray();
                }
            }
            else if (requestType.Equals(typeof(StandingsDataDTO)))
            {
                //var scoringIds = requestIds.Select(x => x[0]).ToArray();
                //var sessionIds = requestIds.Select(x => x.Count() > 0 ? x[1] : 0).ToArray();
                items = GetStandings(requestIds).Cast<TModelDTO>().ToArray();
            }
            else if (requestType.Equals(typeof(IncidentReviewDataDTO)))
            {
                items = GetReviews(requestIds.Select(x => x.FirstOrDefault()).ToArray()).Cast<TModelDTO>().ToArray();
            }
            else
            {
                var rqEntityType = mapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(requestType))?.SourceType;
                if (rqEntityType == null)
                    return null;

                List<TModelDTO> resultItems = new List<TModelDTO>();

                List<object> entities = new List<object>();

                try
                {
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
                }
                catch (Exception e)
                {
                    throw new Exception("Error while getting data from Database.", e);
                }

                try
                {
                    foreach (var entity in entities)
                    {
                        var dto = mapper.MapTo(entity, requestType) as TModelDTO;
                        resultItems.Add(dto);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error while mapping data.", e);
                }
                items = resultItems.ToArray();
            }

            return items;
        }

        public IncidentReviewDataDTO[] GetReviews(long[] keys)
        {
            var mapper = new DTOMapper(DbContext);

            DbContext.Configuration.LazyLoadingEnabled = false;
            var reviewEnties = DbContext.Set<IncidentReviewEntity>().Where(x => keys.Contains(x.ReviewId))
                .Include(x => x.Session)
                .Include(x => x.InvolvedMembers).ToArray();

            DbContext.Set<ReviewCommentEntity>().Where(x => keys.Contains(x.ReviewId))
                .Include(x => x.CommentReviewVotes.Select(y => y.MemberAtFault))
                .Include(x => x.Replies).Load();
            DbContext.Set<AcceptedReviewVoteEntity>().Where(x => keys.Contains(x.ReviewId))
                .Include(x => x.MemberAtFault)
                .Include(x => x.CustomVoteCat)
                .Load();

            DbContext.ChangeTracker.DetectChanges();

            var reviewDtos = reviewEnties.Select(x => mapper.MapTo<IncidentReviewDataDTO>(x)).ToArray();
            DbContext.Configuration.LazyLoadingEnabled = true;

            return reviewDtos;
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

            var entityMapper = new EntityMapper(DbContext) { UserName = UserName, UserId = UserId };
            var dtoMapper = new DTOMapper(DbContext);

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
            var dtoMapper = new DTOMapper(DbContext);

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
                    throw e;
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

            var mapper = new DTOMapper(DbContext);
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

        public ScoredResultDataDTO GetScoredResult(long sessionId, long scoringId)
        {
            var scoredResultData = new ScoredResultDataDTO();

            DbContext.Configuration.LazyLoadingEnabled = false;

            /// Load result and check if recalculation needed
            var session = DbContext.Set<SessionBaseEntity>().Where(x => x.SessionId == sessionId)
                .Include(x => x.SubSessions)
                .Include(x => x.SessionResult)
                .FirstOrDefault();
            IEnumerable<long> sessionIds = new long[] { sessionId };
            if (session.SubSessions?.Count > 0)
            {
                sessionIds = sessionIds.Concat(session.SubSessions.Select(x => x.SessionId));
            }
            var results = DbContext.Set<ResultEntity>().Where(x => sessionIds.Contains(x.ResultId));

            if (results.Count() == 0)
            {
                return new ScoredResultDataDTO()
                {
                    ResultId = sessionId,
                    ScoringId = scoringId
                };
            }
            else if (results.Any(x => x.RequiresRecalculation) || session.SessionResult == null)
            {
                ILeagueActionProvider leagueActionProvider = new LeagueActionProvider(DbContext);
                leagueActionProvider.CalculateScoredResult(sessionId);
            }

            var resultsDataProvider = new ResultsDataProvider(DbContext);
            resultsDataProvider.EagerLoadResult(new long[] { sessionId }, new long[] { scoringId });

            var scoredResultEntity = DbContext.Set<ScoredResultEntity>()
                //.AsNoTracking()
                //.Include(x => x.Result.Session)
                //.Include(x => x.Scoring)
                //.Include(x => x.HardChargers)
                //.Include(x => x.CleanestDrivers)
                //.Include(x => x.Result.RawResults.Select(y => y.Member))
                //.Include(x => x.Result.RawResults.Select(y => y.ScoredResultRows))
                //.Include(x => x.FinalResults.Select(y => y.ResultRow.Member))
                .FirstOrDefault(x => x.ResultId == sessionId && x.ScoringId == scoringId);

            //if (scoredResultEntity == null || scoredResultEntity.Scoring.ShowResults == false)
            //    return new ScoredResultDataDTO()
            //    {
            //        ResultId = sessionId,
            //        ScoringId = scoringId
            //    };
            ////DbContext.Set<ResultEntity>().Where(x => x.ResultId == sessionId)
            ////         .Include(x => x.Session).Load();
            //DbContext.Set<ScoredResultRowEntity>().Where(x => x.ScoredResultId == sessionId && x.ScoringId == scoringId)
            //         .Include(x => x.AddPenalty)
            //         .Include(x => x.ResultRow.Member.Team)
            //         .Include(x => x.ReviewPenalties).Load();
            ////DbContext.Entry(scoredResultEntity).Reference(x => x.Scoring).Load();
            ////DbContext.Entry(scoredResultEntity).Reference(x => x.Result).Query().Include(x => x.Session).Load();
            ////DbContext.Entry(scoredResultEntity).Collection(x => x.FinalResults).Query()
            ////    .Include(x => x.AddPenalty)
            ////    .Include(x => x.ResultRow.Member.Team).Load();
            
            //DbContext.Set<IncidentReviewEntity>()
            //         .Where(x => x.SessionId == sessionId)
            //         .Include(x => x.AcceptedReviewVotes)
            //         .Load();

            ////if (scoredResultRowIds != null)
            ////{
            ////    DbContext.Set<ReviewPenaltyEntity>().Where(x => scoredResultRowIds.Contains(x.ResultRowId))
            ////    .Include(x => x.ReviewVote.MemberAtFault)
            ////    .Include(x => x.ReviewVote.CustomVoteCat)
            ////    .Load();
            ////}

            //if (scoredResultEntity is ScoredTeamResultEntity scoredTeamResultEntity)
            //{
            //    DbContext.Entry(scoredTeamResultEntity).Collection(x => x.TeamResults).Query()
            //        .Include(x => x.ScoredResultRows).Load();
            //}

            DbContext.ChangeTracker.DetectChanges();

            var mapper = new DTOMapper(DbContext);
            scoredResultData = mapper.MapTo<ScoredResultDataDTO>(scoredResultEntity);
            DbContext.Configuration.LazyLoadingEnabled = true;

            return scoredResultData;
        }

        public StandingsDataDTO[] GetStandings(long[] scoringIds)
        {
            return GetStandings(scoringIds.Select(x => new long[] { x }).ToArray());
        }

        public StandingsDataDTO[] GetStandings(long[][] requestIds)
        {
            var mapper = new DTOMapper(DbContext);

            DbContext.Configuration.LazyLoadingEnabled = false;
            List<StandingsDataDTO> responseItems = new List<StandingsDataDTO>();
            List<long> loadedScoringEntityIds = new List<long>();
            foreach (var requestId in requestIds)
            {
                var scoringTableId = requestId[0];
                var sessionId = requestId.Count() > 1 ? requestId[1] : 0;
                ScoringTableEntity scoringTable = null;

                try
                {
                    scoringTable = DbContext.Set<ScoringTableEntity>()
                        .Where(x => x.ScoringTableId == scoringTableId)
                        .Include(x => x.Scorings.Select(y => y.ExtScoringSource))
                        //.Include(x => x.Scorings.Select(y => y.Sessions.Select(z => z.SessionResult)))
                        //.Include(x => x.Scorings.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                        //.Include(x => x.Scorings.Select(y => y.ExtScoringSource.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                        .FirstOrDefault();
                    var loadScoringEntityIds = DbContext.Set<ScoringEntity>().Local.Select(y => y.ScoringId).Except(loadedScoringEntityIds);

                    if (loadScoringEntityIds.Count() > 0)
                    {
                        var loadScorings = DbContext.Set<ScoringEntity>()
                            .Where(x => loadScoringEntityIds.Contains(x.ScoringId))
                            .Include(x => x.Sessions.Select(y => y.SessionResult))
                            .ToList();
                        var loadScoredResults = DbContext.Set<ScoredResultEntity>()
                            .Where(x => loadScoringEntityIds.Contains(x.ScoringId))
                            .Include(x => x.FinalResults.Select(y => y.ResultRow.Member))
                            .Include(x => x.FinalResults.Select(y => y.Team))
                            .ToList();
                    }

                    DbContext.ChangeTracker.DetectChanges();
                }
                catch (Exception e)
                {
                    throw new Exception("Error while getting data from Database.", e);
                }

                try
                {
                    if (scoringTable != null)
                    {
                        StandingsEntity standings;
                        if (sessionId == 0)
                            standings = scoringTable.GetSeasonStandings(DbContext);
                        else
                        {
                            var scoringSession = scoringTable.GetAllSessions().SingleOrDefault(x => x.SessionId == sessionId);
                            if (scoringSession == null)
                            {
                                var session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
                                scoringSession = scoringTable.GetAllSessions().LastOrDefault(x => x.Date <= session?.Date);
                            }
                            standings = scoringTable.GetSeasonStandings(scoringSession, DbContext);
                        }
                        var standingsDTO = mapper.MapTo<StandingsDataDTO>(standings);
                        responseItems.Add(standingsDTO);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error while mapping data.", e);
                }
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