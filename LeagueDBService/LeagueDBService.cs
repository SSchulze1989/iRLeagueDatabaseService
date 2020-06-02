using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Mapper;
using System.Data.Entity;
using iRLeagueDatabase.DataTransfer.Messages;
//using AutoMapper;
//using AutoMapper.Collection;
//using AutoMapper.EquivalencyExpression;

namespace LeagueDBService
{
    // HINWEIS: Mit dem Befehl "Umbenennen" im Menü "Umgestalten" können Sie den Klassennamen "Service1" sowohl im Code als auch in der Konfigurationsdatei ändern.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerCall)]
    public class LeagueDBService : ILeagueDBService, IDisposable
    {
        //AppProfile MapperProfile { get; }

        //MapperConfiguration MapperConfiguration { get; set; }

        private string DatabaseName { get; set; }

        public LeagueDBService()
        {
            DatabaseName = "TestDatabase";
            //MapperProfile = new AppProfile();
            //MapperConfiguration = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddCollectionMappers();
            //    cfg.AddProfile(MapperProfile);
            //});
        }

        public LeagueDBService(string databaseName) : this()
        {
            DatabaseName = databaseName;
        }

        public void SetDatabaseName(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public void CleanUpSessions()
        {
            //using (var leagueDb = new LeagueDbContext(DatabaseName))
            //{
            //    var Sessions = leagueDb.Sessions.ToArray();
            //    foreach (var session in Sessions)
            //    {
            //        if (session.Schedule == null)
            //        {
            //            leagueDb.Sessions.Remove(session);
            //        }
            //    }
            //    leagueDb.SaveChanges();
            //}
        }

        public string Test(string name)
        {
            return "Hallo " + name + "!";
        }

        public string TestDB()
        {
            string result;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                result = leagueDb.Seasons.First().SeasonName;
            }
            return result;
        }

        public LeagueMemberDataDTO GetMember(long memberId)
        {
            LeagueMemberDataDTO leagueMember;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var memberEntity = leagueDb.Members.SingleOrDefault(x => x.MemberId == memberId);
                if (memberEntity != null)
                {
                    //var mapper = MapperConfiguration.CreateMapper();

                    //leagueMember = mapper.Map<LeagueMemberDataDTO>(memberEntity);
                    var mapper = new DTOMapper();
                    leagueMember = mapper.MapToMemberDataDTO(memberEntity);
                }
                else
                {
                    leagueMember = null;
                }
                return leagueMember;
            }
        }

        public List<SeasonDataDTO> GetSeasons(long[] seasonIds = null)
        {
            //IQueryable<Season> seasonEntities;
            IEnumerable<SeasonEntity> seasonEntities;
            List<SeasonDataDTO> seasonDTOs = new List<SeasonDataDTO>();

            //var mapper = MapperConfiguration.CreateMapper();

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                // Deprecated becaus Automapper ignores include statements with ProjectTo()
                //if (seasonIds == null || seasonIds == new int[0])
                //{
                //    seasonEntities = leagueDb.Seasons.AsQueryable();
                //}
                //else
                //{
                //    seasonEntities = leagueDb.Seasons.Where(x => seasonIds.Contains(x.SeasonId));
                //}

                //seasonDTOs = mapper.ProjectTo<SeasonDataDTO>(seasonEntities).ToList();
                var mapper = new DTOMapper();

                if (seasonIds == null || seasonIds == new long[0])
                {
                    seasonEntities = leagueDb.Seasons.ToList();
                }
                else
                {
                    seasonEntities = leagueDb.Seasons.Where(x => seasonIds.Contains(x.SeasonId)).ToList();
                }

                foreach (var seasonEntity in seasonEntities)
                {
                    //seasonDTOs.Add(mapper.Map<SeasonDataDTO>(seasonEntity));
                    seasonDTOs.Add(mapper.MapToSeasonDataDTO(seasonEntity));
                }
            }

            return seasonDTOs;
        }

        public SeasonDataDTO PutSeason(SeasonDataDTO seasonData)
        {
            if (seasonData == null)
                return null;

            SeasonDataDTO returnData;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                SeasonEntity seasonEntity;

                if (leagueDb.Seasons.Any(x => x.SeasonId == seasonData.SeasonId))
                {
                    seasonEntity = leagueDb.Seasons.Find(seasonData.SeasonId);
                    //mapper.Map(seasonData, seasonEntity);
                    entityMapper.MapToSeasonEntity(seasonData, seasonEntity);
                }
                else
                {
                    seasonEntity = entityMapper.MapToSeasonEntity(seasonData);
                    seasonEntity = leagueDb.Seasons.Add(seasonEntity);
                }

                leagueDb.SaveChanges();
                
                seasonEntity = leagueDb.Seasons.Find(seasonEntity.SeasonId);
                //returnData = Mapper.Map<SeasonDataDTO>(seasonEntity);
                returnData = dtoMapper.MapToSeasonDataDTO(seasonEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public List<LeagueMemberDataDTO> GetMembers(long[] memberIds = null)
        {
            IQueryable<LeagueMemberEntity> memberEntities;
            List<LeagueMemberDataDTO> memberDTOs;

            //var mapper = MapperConfiguration.CreateMapper();

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var mapper = new DTOMapper();

                if (memberIds == null || memberIds == new long[0])
                {
                    memberEntities = leagueDb.Members.AsQueryable();
                }
                else
                {
                    memberEntities = leagueDb.Members.Where(x => memberIds.Contains(x.MemberId));
                }

                //memberDTOs = mapper.ProjectTo<LeagueMemberDataDTO>(memberEntities).ToList();
                memberDTOs = memberEntities.ToArray().Select(x => mapper.MapToMemberDataDTO(x, null)).ToList();
            }

            return memberDTOs;
        }

        public LeagueMemberDataDTO[] UpdateMemberList(LeagueMemberDataDTO[] members)
        {
            if (members == null || members.Count() == 0)
                return null;

            LeagueMemberDataDTO[] returnData;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                var entityMapper = new EntityMapper(leagueDb);
                foreach (var memberData in members)
                {
                    //MapperProfile.DbContext = leagueDb;

                    LeagueMemberEntity memberEntity;

                    //Put review to Db
                    if (leagueDb.Members.Any(x => x.MemberId == memberData.MemberId) && memberData.MemberId != 0)
                    {
                        memberEntity = leagueDb.Members.Find(memberData.MemberId);
                        //mapper.Map(memberData, memberEntity);
                        entityMapper.MapToMemberEntity(memberData, memberEntity);
                    }
                    else
                    {
                        //memberEntity = mapper.Map<LeagueMemberEntity>(memberData);
                        memberEntity = entityMapper.MapToMemberEntity(memberData);
                        leagueDb.Members.Add(memberEntity);
                    }
                }
                leagueDb.SaveChanges();

                //Get review object to return

                returnData = GetMembers().ToArray();
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public LeagueMemberDataDTO GetLastMember()
        {
            long lastMemberId = 0;

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var member = leagueDb.Members.ToList().Last();
                lastMemberId = member.MemberId;
            }

            return GetMember(lastMemberId);
        }

        public LeagueMemberDataDTO PutMember(LeagueMemberDataDTO memberData)
        {
            if (memberData == null)
                return null;

            LeagueMemberDataDTO returnData;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);

                LeagueMemberEntity memberEntity;

                //Put review to Db
                if (leagueDb.Members.Any(x => x.MemberId == memberData.MemberId))
                {
                    memberEntity = leagueDb.Members.Find(memberData.MemberId);
                    //mapper.Map(memberData, memberEntity);
                    entityMapper.MapToMemberEntity(memberData, memberEntity);
                }
                else
                {
                    //memberEntity = mapper.Map<LeagueMemberEntity>(memberData);
                    memberEntity = entityMapper.MapToMemberEntity(memberData);
                    leagueDb.Members.Add(memberEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                memberEntity = leagueDb.Members.Find(memberData.MemberId);
                //returnData = mapper.Map<LeagueMemberDataDTO>(memberEntity);
                var dtoMapper = new DTOMapper();
                returnData = dtoMapper.MapToMemberDataDTO(memberEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public SeasonDataDTO GetSeason(long seasonId)
        {
            SeasonDataDTO season = null;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperConfiguration.CreateMapper();
                var mapper = new DTOMapper();
                var seasonEntity = leagueDb.Seasons.SingleOrDefault(x => x.SeasonId == seasonId);
                //season = mapper.Map<SeasonDataDTO>(seasonEntity);
                season = mapper.MapToSeasonDataDTO(seasonEntity);
            }

            return season;
        }

        public IncidentReviewDataDTO GetReview(long reviewId)
        {
            //var mapper = MapperConfiguration.CreateMapper();
            IncidentReviewDataDTO review = null;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var mapper = new DTOMapper();
                var reviewEntitiy = leagueDb.Set<IncidentReviewEntity>().Find(reviewId);
                //review = mapper.Map<IncidentReviewDataDTO>(reviewEntitiy);
                review = mapper.MapToReviewDataDTO(reviewEntitiy);
            }
            return review;
        }
        
        public IncidentReviewDataDTO PutReview(IncidentReviewDataDTO review)
        {
            if (review == null)
                return null;

            IncidentReviewDataDTO returnReview;
            //var mapper = MapperConfiguration.CreateMapper();

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                IncidentReviewEntity reviewEntity;
                var reviewSet = leagueDb.Set<IncidentReviewEntity>();

                //Put review to Db
                if (reviewSet.Any(x => x.ReviewId == review.ReviewId))
                {
                    reviewEntity = reviewSet.Find(review.ReviewId);
                    //var config = new MapperConfiguration(cfg => {
                    //    cfg.CreateMap<IncidentReviewDTO, IncidentReview>()
                    //        .ForMember(dest => dest.MemberAtFault, map => map.MapFrom((source, dest) => leagueDb.Members.Where(x => x.MemberId == source.MemberAtFaultId).FirstOrDefault()))
                    //        .MapOnlyIfChanged();
                    //});
                    //mapper.Map(review, reviewEntity);
                    entityMapper.MapToReviewEntity(review, reviewEntity);
                }
                else
                {
                    //reviewEntity = mapper.Map<IncidentReviewEntity>(review);
                    reviewEntity = entityMapper.MapToReviewEntity(review);
                    reviewSet.Add(reviewEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                reviewEntity = reviewSet.Find(review.ReviewId);
                //returnReview = mapper.Map<IncidentReviewDataDTO>(reviewEntity);
                returnReview = dtoMapper.MapToReviewDataDTO(reviewEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnReview;
        }

        public CommentDataDTO GetComment(long commentId)
        {
            //var mapper = MapperConfiguration.CreateMapper();
            CommentDataDTO comment = null;

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var mapper = new DTOMapper();
                var commentSet = leagueDb.Set<CommentBaseEntity>();
                var commentEntity = commentSet.Where(x => x.CommentId == commentId).FirstOrDefault();

                if (commentEntity is ReviewCommentEntity reviewComment)
                {
                    //comment = mapper.Map<ReviewCommentDataDTO>(commentEntity);
                    comment = mapper.MapToReviewCommentDataDTO(reviewComment);
                }
                else
                {
                    //comment = mapper.Map<CommentDataDTO>(commentEntity);
                    comment = mapper.MapToCommentDataDTO(commentEntity);
                }
            }
            return comment;
        }

        public CommentDataDTO PutComment(ReviewCommentDataDTO comment)
        {
            if (comment == null)
                return null;

            CommentDataDTO returnComment;
            //var mapper = MapperConfiguration.CreateMapper();

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                CommentBaseEntity commentEntity;
                var commentSet = leagueDb.Set<CommentBaseEntity>();

                //Put review to Db
                if (commentSet.Any(x => x.CommentId == comment.CommentId))
                {
                    commentEntity = commentSet.Find(comment.CommentId);
                    //mapper.Map(comment, commentEntity);
                    entityMapper.MapToCommentBaseEntity(comment, commentEntity);
                }
                else
                {
                    //commentEntity = mapper.Map<CommentBaseEntity>(comment);
                    commentEntity = entityMapper.MapToCommentBaseEntity(comment);
                    commentSet.Add(commentEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                commentEntity = commentSet.Find(comment.CommentId);
                //returnComment = mapper.Map<CommentDataDTO>(commentEntity);
                returnComment = dtoMapper.MapToCommentDataDTO(commentEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnComment;
        }

        public SessionDataDTO GetSession(long sessionId)
        {
            SessionDataDTO sessionData;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var sessionEntity = leagueDb.Set<SessionBaseEntity>().Find(sessionId);
                if (sessionEntity != null)
                {
                    //var mapper = MapperConfiguration.CreateMapper();
                    var mapper = new DTOMapper();

                    //sessionData = mapper.Map<SessionDataDTO>(memberEntity);
                    sessionData = mapper.MapToSessionDataDTO(sessionEntity);
                }
                else
                {
                    sessionData = null;
                }
                return sessionData;
            }
        }

        public SessionDataDTO PutSession(SessionDataDTO sessionData)
        {
            SessionDataDTO returnData = null;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                SessionBaseEntity sessionEntity;
                var sessionSet = leagueDb.Set<SessionBaseEntity>();

                //Put review to Db
                if (sessionSet.Any(x => x.SessionId == sessionData.SessionId))
                {
                    sessionEntity = sessionSet.SingleOrDefault(x => x.SessionId == sessionData.SessionId);
                    //mapper.Map(sessionData, sessionEntity);
                    entityMapper.MapToSessionBaseEntity(sessionData, sessionEntity);
                }
                else
                {
                    //sessionEntity = mapper.Map<SessionBaseEntity>(sessionData);
                    sessionEntity = entityMapper.MapToSessionBaseEntity(sessionData);
                    sessionSet.Add(sessionEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                //returnData = mapper.Map<SessionDataDTO>(sessionEntity);
                returnData = dtoMapper.MapToSessionDataDTO(sessionEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public ScheduleDataDTO GetSchedule(long scheduleId)
        {
            ScheduleDataDTO scheduleData;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var scheduleEntity = leagueDb.Set<ScheduleEntity>().Find(scheduleId);
                if (scheduleEntity != null)
                {
                    //var mapper = MapperConfiguration.CreateMapper();
                    var mapper = new DTOMapper();

                    //scheduleData = mapper.Map<ScheduleDataDTO>(memberEntity);
                    scheduleData = mapper.MapToScheduleDataDTO(scheduleEntity);
                }
                else
                {
                    scheduleData = null;
                }
                return scheduleData;
            }
        }

        public ScheduleDataDTO PutSchedule(ScheduleDataDTO scheduleData)
        {
            ScheduleDataDTO returnData = null;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                ScheduleEntity scheduleEntity;
                var scheduleSet = leagueDb.Set<ScheduleEntity>();

                //Put review to Db
                if (scheduleSet.Any(x => x.ScheduleId == scheduleData.ScheduleId))
                {
                    scheduleEntity = scheduleSet.Find(scheduleData.ScheduleId);
                    //mapper.Map(scheduleData, scheduleEntity);
                    entityMapper.MapToScheduleEntity(scheduleData, scheduleEntity);   
                }
                else
                {
                    //scheduleEntity = mapper.Map<ScheduleEntity>(scheduleData);
                    scheduleEntity = entityMapper.MapToScheduleEntity(scheduleData);
                    scheduleSet.Add(scheduleEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                scheduleEntity = scheduleSet.Find(scheduleData.ScheduleId);
                //returnData = mapper.Map<ScheduleDataDTO>(scheduleEntity);
                returnData = dtoMapper.MapToScheduleDataDTO(scheduleEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public List<ScheduleDataDTO> GetSchedules(long[] scheduleIds = null)
        {
            IEnumerable<ScheduleEntity> scheduleEntities;
            List<ScheduleDataDTO> scheduleDTOs = new List<ScheduleDataDTO>();

            //var mapper = MapperConfiguration.CreateMapper();

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var mapper = new DTOMapper();
                var scheduleSet = leagueDb.Set<ScheduleEntity>();

                if (scheduleIds == null || scheduleIds == new long[0])
                {
                    scheduleEntities = scheduleSet.ToArray();
                }
                else
                {
                    scheduleEntities = scheduleSet.Where(x => scheduleIds.Contains(x.ScheduleId)).ToArray();
                }

                //scheduleDTOs = mapper.ProjectTo<ScheduleDataDTO>(scheduleEntities).ToList();
                //mapper.Map(scheduleEntities, scheduleDTOs);
                scheduleDTOs = scheduleEntities.Select(x => mapper.MapToScheduleDataDTO(x, null)).ToList();
            }

            return scheduleDTOs;
        }

        public ResultDataDTO GetResult(long resultId)
        {
            ResultDataDTO resultData;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var resultEntity = leagueDb.Set<ResultEntity>().Find(resultId);
                if (resultEntity != null)
                {
                    //var mapper = MapperConfiguration.CreateMapper();
                    var mapper = new DTOMapper();

                    //resultData = mapper.Map<ResultDataDTO>(memberEntity);
                    resultData = mapper.MapToResulDataDTO(resultEntity);
                }
                else
                {
                    resultData = null;
                }
                return resultData;
            }
        }

        public ResultDataDTO PutResult(ResultDataDTO resultData)
        {
            ResultDataDTO returnData = null;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                ResultEntity resultEntity;
                var resultSet = leagueDb.Set<ResultEntity>();

                //Put review to Db
                if (resultSet.Any(x => x.ResultId == resultData.ResultId))
                {
                    resultEntity = resultSet.Find(resultData.ResultId);
                    //mapper.Map(resultData, resultEntity);
                    entityMapper.MapToResultEntity(resultData, resultEntity);
                }
                else
                {
                    //resultEntity = mapper.Map<ResultEntity>(resultData);
                    resultEntity = entityMapper.MapToResultEntity(resultData);
                    resultSet.Add(resultEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                resultEntity = resultSet.Find(resultData.ResultId);
                //returnData = mapper.Map<ResultDataDTO>(resultEntity);
                returnData = dtoMapper.MapToResulDataDTO(resultEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public ScoringDataDTO PutScoring(ScoringDataDTO scoringData)
        {
            ScoringDataDTO returnData = null;
            //var mapper = MapperConfiguration.CreateMapper();

            using (LeagueDbContext leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var mapper = MapperHelper.GetEntityMapper(leagueDb);
                //MapperProfile.DbContext = leagueDb;
                var entityMapper = new EntityMapper(leagueDb);
                var dtoMapper = new DTOMapper();

                ScoringEntity scoringEntity;
                var scoringSet = leagueDb.Set<ScoringEntity>();

                //Put review to Db
                if (scoringSet.Any(x => x.ScoringId == scoringData.ScoringId))
                {
                    scoringEntity = scoringSet.Find(scoringData.ScoringId);
                    //mapper.Map(scoringData, scoringEntity);
                    entityMapper.MapToScoringEntity(scoringData, scoringEntity);
                }
                else
                {
                    //scoringEntity = mapper.Map<ScoringEntity>(scoringData);
                    scoringEntity = entityMapper.MapToScoringEntity(scoringData);
                    scoringSet.Add(scoringEntity);
                }
                leagueDb.SaveChanges();

                //Get review object to return
                scoringEntity = scoringSet.Find(scoringData.ScoringId);
                //returnData = mapper.Map<ScoringDataDTO>(scoringEntity);
                returnData = dtoMapper.MapToScoringDataDTO(scoringEntity);
                //MapperProfile.DbContext = null; leagueDb.Dispose();
            }

            return returnData;
        }

        public ScoringDataDTO GetScoring(long scoringId)
        {
            ScoringDataDTO scoringData;
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var scoringEntity = leagueDb.Set<ScoringEntity>().Find(scoringId);
                if (scoringEntity != null)
                {
                    //var mapper = MapperConfiguration.CreateMapper();
                    var mapper = new DTOMapper();

                    //scoringData = mapper.Map<ScoringDataDTO>(memberEntity);
                    scoringData = mapper.MapToScoringDataDTO(scoringEntity);
                }
                else
                {
                    scoringData = null;
                }
                return scoringData;
            }
        }



        public void CalculateScoredResults(long sessionId)
        {
            IEnumerable<ScoringEntity> scorings;

            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                var session = leagueDb.Set<SessionBaseEntity>().Find(sessionId);
                scorings = session.Scorings;

                foreach(var scoring in scorings)
                {
                    scoring.CalculateResults(session.SessionId, leagueDb);
                }
                leagueDb.SaveChanges();
            }
        }
        public ScoredResultDataDTO GetScoredResult(long sessionId, long scoringId)
        {
            ScoredResultDataDTO scoredResultData = new ScoredResultDataDTO();
            using (var leagueDb = new LeagueDbContext(DatabaseName))
            {
                //var scoredResultRowsEntity = leagueDb.Set<ScoredResultRowEntity>().Where(x => x.ResultId == sessionId && x.ScoringId == scoringId).ToArray().AsEnumerable();
                var scoredResultEntity = leagueDb.Set<ScoredResultEntity>().Find(sessionId, scoringId);

                var scoredResultRowsEntity = new ScoredResultRowEntity[0];
                if (scoredResultEntity != null)
                    scoredResultRowsEntity = scoredResultEntity.FinalResults.ToArray();
                //var mapper = MapperConfiguration.CreateMapper();
                var mapper = new DTOMapper();

                //ScoredResultData.Scoring = mapper.Map<ScoringDataDTO>(GetScoring(scoringId));
                scoredResultData.Scoring = mapper.MapToScoringInfoDTO(leagueDb.Set<ScoringEntity>().Find(scoringId));

                var result = leagueDb.Set<ResultEntity>().Find(sessionId);
                if (result != null)
                {
                    //mapper.Map(result, ScoredResultData);
                    var tmp = mapper.MapToResulDataDTO(result, scoredResultData);
                }
                if (scoredResultRowsEntity.Count() > 0)
                {
                    //ScoredResultData.ScoredResults = mapper.Map<IEnumerable<ScoredResultRowDataDTO>>(scoredResultRowsEntity);
                    scoredResultData.FinalResults = scoredResultRowsEntity.Select(x => mapper.MapToScoredResultRowDataDTO(x, null)).OrderBy(x => x.FinalPosition);
                }
                else
                {
                    scoredResultData.FinalResults = new ScoredResultRowDataDTO[0];
                }

                return scoredResultData;
            }
        }

        public StandingsRowDTO[] GetSeasonStandings(long seasonId, long? lastSessionId = null)
        {
            //SeasonEntity seasonEntity;
            List<StandingsRowDTO> standings = new List<StandingsRowDTO>();

            //using (var leagueDb = new LeagueDbContext(DatabaseName))
            //{
            //    seasonEntity = leagueDb.Seasons.Find(seasonId);

            //    if (seasonEntity == null)
            //        return null;

            //    IEnumerable<ResultEntity> results = leagueDb.Set<ResultEntity>().Where(x => x.Session.Schedule.Season.SeasonId == seasonId)
            //        .OrderBy(x => x.Session.Date);

            //    if (lastSessionId != null)
            //    {
            //        var lastSession = results.Select(x => x.Session).SingleOrDefault(x => x.SessionId == lastSessionId);
            //        results = results.Where(x => x.Session.Date <= lastSession.Date).OrderBy(x => x.Session.Date);
            //    }

            //    Func<ResultRowEntity, int> getPoints = new Func<ResultRowEntity, int>(x => x.TotalPoints);
            //    int racesCounted = 8;

            //    //Calculate standings
            //    // Get different drivers in season
            //    var drivers = results.Select(x => x.RawResults.Select(y => y.Member)).Aggregate((x, y) => x.Concat(y)).Distinct();
            //    var lastRace = results.OrderBy(x => x.Session.Date).LastOrDefault();

            //    Dictionary<StandingsRowDTO, IEnumerable<ResultRowEntity>> standingsList = new Dictionary<StandingsRowDTO, IEnumerable<ResultRowEntity>>();
            //    Dictionary<StandingsRowDTO, ResultRowEntity> lastRaceList = new Dictionary<StandingsRowDTO, ResultRowEntity>();

            //    foreach (var driver in drivers)
            //    {
            //        var driverResults = results.Where(x => x.RawResults.Exists(y => y.Member.MemberId == driver.MemberId)).OrderBy(x => x.Session.Date).ToList();
            //        var driverResultRows = driverResults.Select(x => x.RawResults?.SingleOrDefault(y => y.Member.MemberId == driver.MemberId)).ToList();
            //        var driverResultsCounted = driverResultRows.OrderBy(x => x.TotalPoints).Where(x => x.ResultId != lastRace.ResultId).Take(racesCounted);
            //        var driverLastRaceResult = driverResultRows.SingleOrDefault(x => x.Result.Session.Date == lastRace.Session.Date);

            //        var driverStandingsRow = new StandingsRowDTO()
            //        {
            //            MemberId = driver.MemberId,
            //            Name = driver.Fullname,
            //            Wins = driverResultRows.Select(x => x.FinalPosition).Where(x => x == 1).Count(),
            //            Top3 = driverResultRows.Select(x => x.FinalPosition).Where(x => x <= 3).Count(),
            //            Top5 = driverResultRows.Select(x => x.FinalPosition).Where(x => x <= 5).Count(),
            //            Top10 = driverResultRows.Select(x => x.FinalPosition).Where(x => x <= 10).Count(),
            //            Top15 = driverResultRows.Select(x => x.FinalPosition).Where(x => x <= 15).Count(),
            //            Top20 = driverResultRows.Select(x => x.FinalPosition).Where(x => x <= 20).Count(),
            //            Poles = driverResultRows.Select(x => x.StartPosition).Where(x => x == 1).Count(),
            //            PenaltyPoints = driverResultRows.Select(x => x.PenaltyPoints).Aggregate((x, y) => x + y),
            //            FastestLaps = driverResults.Select(x => x.RawResults.OrderBy(y => y.FastestLapTime).First()).Where(x => x.Member.MemberId == driver.MemberId).Count(),
            //            RacesParticipated = driverResults.Count(),
            //            RacesCounted = driverResultsCounted.Count(),
            //            Change = 0,
            //            PointsChange = 0
            //        };

            //        KeyValuePair<StandingsRowDTO, IEnumerable<ResultRowEntity>> standingsPair = new KeyValuePair<StandingsRowDTO, IEnumerable<ResultRowEntity>>(driverStandingsRow, driverResultsCounted);
            //        KeyValuePair<StandingsRowDTO, ResultRowEntity> lastRacePair = new KeyValuePair<StandingsRowDTO, ResultRowEntity>(driverStandingsRow, driverLastRaceResult);
            //        standingsList.Add(standingsPair.Key, standingsPair.Value);
            //        lastRaceList.Add(lastRacePair.Key, lastRacePair.Value);
            //    }

            //    standings = CalcPoints(standingsList, getPoints).ToList();
            //    standings = CalcPositions(standings).ToList();

            //    foreach (var key in standingsList.Keys)
            //    {
            //        if (lastRaceList[key] != null)
            //        {
            //            standingsList[key] = standingsList[key].Take(racesCounted - 1).Concat(new ResultRowEntity[] { lastRaceList[key] });
            //        }
            //    }

            //    standings = CalcPoints(standingsList, getPoints).ToList();
            //    standings = CalcPositions(standings).OrderBy(x => x.Pos).ToList();
            //}

            return standings.ToArray();
        }

        private IEnumerable<StandingsRowDTO> CalcPoints(IEnumerable<KeyValuePair<StandingsRowDTO, IEnumerable<ResultRowEntity>>> results, Func<ResultRowEntity, int> getPoints)
        {
            foreach (var entry in results)
            {
                var standingsRow = entry.Key;
                var resultRows = entry.Value;

                standingsRow.PointsChange = 0;

                foreach (var resultRow in resultRows)
                {
                    var racePoints = getPoints(resultRow);
                    standingsRow.Points += racePoints;
                    standingsRow.PointsChange = racePoints;
                }
            }
            return results.Select(x => x.Key);
        }

        private IEnumerable<StandingsRowDTO> CalcPositions(IEnumerable<StandingsRowDTO> standingsRows)
        {
            standingsRows = standingsRows.OrderBy(x => x.PenaltyPoints).OrderByDescending(x => x.Top3).OrderByDescending(x => x.Wins).OrderByDescending(x => x.Points);

            for (int i = 0; i < standingsRows.Count(); i++)
            {
                var row = standingsRows.ElementAt(i);
                row.Change = row.Pos - (i + 1);
                row.Pos = i + 1;
            }

            return standingsRows;
        }

        public StandingsRowDTO[] GetTeamStandings(long seasonId, long? lastSessionId)
        {
            return null;
        }

        public GETItemsResponseMessage DatabaseGET(GETItemsRequestMessage requestMsg)
        {
            GETItemsResponseMessage responseMsg = new GETItemsResponseMessage();

            if (requestMsg == null)
                return null;

            var searchNames = new string[]
            {
                "iRLeagueDatabase.DataTransfer.",
                "iRLeagueDatabase.DataTransfer.Members.",
                "iRLeagueDatabase.DataTransfer.Results.",
                "iRLeagueDatabase.DataTransfer.Reviews.",
                "iRLeagueDatabase.DataTransfer.Sessions."
            };


            var dbName = requestMsg.databaseName;
            //var rqType = Type.GetType(nSpace + requestMsg.requestItemType);
            Type rqType = null;
            foreach (var name in searchNames)
            {
                rqType = Type.GetType(name + requestMsg.requestItemType);

                if (rqType != null)
                    break;
            }
            object[][] requestIds = requestMsg.requestItemIds?.Select(x => x.Cast<object>().ToArray()).ToArray();

            using (var dbContext = new LeagueDbContext(dbName))
            {
                var mapper = new DTOMapper();
                responseMsg.databaseName = dbName;

                if (rqType.Equals(typeof(ScoredResultDataDTO)))
                {
                    responseMsg.items = requestMsg.requestItemIds.Select(x => GetScoredResult(x[0], x[1])).ToArray();
                }
                else
                {
                    var rqEntityType = mapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(rqType))?.SourceType;
                    if (rqEntityType == null)
                        return null;

                    List<MappableDTO> resultItems = new List<MappableDTO>();

                    List<object> entities = new List<object>();

                    if (requestIds != null)
                    {
                        foreach (var keys in requestIds)
                        {
                            object entity = dbContext.Set(rqEntityType).Find(keys);
                            if (entity == null)
                                throw new Exception("Entity not found in Database - Type: " + rqEntityType.Name + " || keys: { " + keys.Select(x => x.ToString()).Aggregate((x, y) => ", ") + " }");

                            entities.Add(entity);
                        }
                    }
                    else
                    {
                        entities = dbContext.Set(rqEntityType).ToListAsync().Result;
                    }

                    foreach (var entity in entities)
                    {
                        var dto = mapper.MapTo(entity, rqType) as MappableDTO;
                        resultItems.Add(dto);
                    }
                    responseMsg.items = resultItems.ToArray();
                }
            };

            responseMsg.status = "success";

            //GC.Collect();
            return responseMsg;
        }

        public ResponseMessage MessageTest(RequestMessage request)
        {
            var response = new ResponseMessage()
            {
                databaseName = request.databaseName,
                status = "success"
            };

            return response;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LeagueDBService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public POSTItemsResponseMessage DatabasePOST(POSTItemsRequestMessage requestMsg)
        {
            POSTItemsResponseMessage responseMsg = new POSTItemsResponseMessage();

            if (requestMsg == null)
                return null;

            var searchNames = new string[]
            {
                "iRLeagueDatabase.DataTransfer.",
                "iRLeagueDatabase.DataTransfer.Members.",
                "iRLeagueDatabase.DataTransfer.Results.",
                "iRLeagueDatabase.DataTransfer.Reviews.",
                "iRLeagueDatabase.DataTransfer.Sessions."
            };


            var dbName = requestMsg.databaseName;
            //var rqType = Type.GetType(nSpace + requestMsg.requestItemType);
            Type rqType = null;
            foreach (var name in searchNames)
            {
                rqType = Type.GetType(name + requestMsg.requestItemType);

                if (rqType != null)
                    break;
            }

            using (var dbContext = new LeagueDbContext(dbName))
            {
                var entityMapper = new EntityMapper(dbContext);
                var dtoMapper = new DTOMapper();
                responseMsg.databaseName = dbName;

                var rqEntityType = dtoMapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(rqType))?.SourceType;
                if (rqEntityType == null)
                    return null;

                var dbSet = dbContext.Set(rqEntityType);

                List<MappableDTO> resultItems = new List<MappableDTO>();
                foreach (object item in requestMsg.items)
                {
                    object entity = dbSet.Create();
                    entity = entityMapper.MapTo(item, entity, rqType, rqEntityType);
                    dbSet.Add(entity);

                    dbContext.SaveChanges();

                    var dto = dtoMapper.MapTo(entity, rqType) as MappableDTO;
                    resultItems.Add(dto);
                }
                responseMsg.items = resultItems.ToArray();
            };

            responseMsg.status = "success";
            return responseMsg;
        }

        public PUTItemsResponseMessage DatabasePUT(PUTItemsRequestMessage requestMsg)
        {
            PUTItemsResponseMessage responseMsg = new PUTItemsResponseMessage();

            if (requestMsg == null)
                return null;

            var searchNames = new string[]
            {
                "iRLeagueDatabase.DataTransfer.",
                "iRLeagueDatabase.DataTransfer.Members.",
                "iRLeagueDatabase.DataTransfer.Results.",
                "iRLeagueDatabase.DataTransfer.Reviews.",
                "iRLeagueDatabase.DataTransfer.Sessions."
            };


            var dbName = requestMsg.databaseName;
            //var rqType = Type.GetType(nSpace + requestMsg.requestItemType);
            Type rqType = null;
            foreach (var name in searchNames)
            {
                rqType = Type.GetType(name + requestMsg.requestItemType);

                if (rqType != null)
                    break;
            }

            using (var dbContext = new LeagueDbContext(dbName))
            {
                var entityMapper = new EntityMapper(dbContext);
                var dtoMapper = new DTOMapper();
                responseMsg.databaseName = dbName;

                var rqEntityType = dtoMapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(rqType))?.SourceType;
                if (rqEntityType == null)
                    return null;

                List<MappableDTO> resultItems = new List<MappableDTO>();
                foreach (object item in requestMsg.items)
                {
                    object entity = entityMapper.MapTo(item, null, rqType, rqEntityType);
                    dbContext.SaveChanges();
                    var dto = dtoMapper.MapTo(entity, rqType) as MappableDTO;
                    resultItems.Add(dto);
                }
                responseMsg.items = resultItems.ToArray();
            };

            responseMsg.status = "success";
            return responseMsg;
        }

        public DELItemsResponseMessage DatabaseDEL(DELItemsRequestMessage requestMsg)
        {
            DELItemsResponseMessage responseMsg = new DELItemsResponseMessage();
            var dbName = requestMsg.databaseName;
            responseMsg.success = false;

            if (requestMsg == null)
                return null;

            var requestItemIds = requestMsg.requestItemIds.Select(x => x.Cast<object>().ToArray()).ToArray();

            var searchNames = new string[]
            {
                "iRLeagueDatabase.DataTransfer.",
                "iRLeagueDatabase.DataTransfer.Members.",
                "iRLeagueDatabase.DataTransfer.Results.",
                "iRLeagueDatabase.DataTransfer.Reviews.",
                "iRLeagueDatabase.DataTransfer.Sessions."
            };

            Type rqType = null;
            foreach (var name in searchNames)
            {
                rqType = Type.GetType(name + requestMsg.requestItemType);

                if (rqType != null)
                    break;
            }

            using (var dbContext = new LeagueDbContext(dbName))
            {
                var mapper = new DTOMapper();
                var rqEntityType = mapper.GetTypeMaps().FirstOrDefault(x => x.TargetType.Equals(rqType))?.SourceType;
                if (rqEntityType == null)
                    throw new Exception("No typemap for " + rqType.Name + " found");

                foreach (var keys in requestItemIds)
                {
                    var entity = dbContext.Set(rqEntityType).Find(keys) as MappableEntity;

                    if (entity != null)
                    {
                        //dbContext.Set(rqEntityType).Remove(entity);
                        entity.Delete(dbContext);
                        responseMsg.status = "deleted";
                        responseMsg.success = true;
                    }
                }
                dbContext.SaveChanges();
            };

            return responseMsg;
        }
        
    }
}
