using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.DataAccess.Mapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace iRLeagueDatabase.DataAccess.Provider
{
    /// <summary>
    /// Data access class for retrieving review convenience data from League database
    /// </summary>
    public class ResultsDataProvider : DataProviderBase, IResultsDataProvider
    {
        public ResultsDataProvider(IProviderContext<LeagueDbContext> context) : base(context)
        {
        }

        /// <summary>
        /// Get the results of a single session
        /// If <paramref name="sessionId"/> == 0 the lates session with attached result is used
        /// </summary>
        /// <param name="sessionId">Id of the session</param>
        /// <returns>Convenience DTO for all session results</returns>
        public SessionResultsDTO GetResultsFromSession(long sessionId, bool includeRawResults = false, ScoredResultDataDTO[] scoredResults = null)
        {
            var mapper = new DTOMapper(DbContext);

            // get session entity from database
            SessionBaseEntity session;
            if (sessionId == 0)
            {
                // if sessionId is 0 get latest session with result
                session = DbContext.Set<SessionBaseEntity>()
                    .Where(x => x.SessionResult != null && x.SessionType != iRLeagueManager.Enums.SessionType.Heat)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
            }
            else
            {
                // else get session from sessionId
                session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
            }

            if (session == null)
            {
                return new SessionResultsDTO() { SessionId = sessionId };
            }
            sessionId = session.SessionId;

            // get session race number
            int raceNr = 0;
            if (session.SessionType == iRLeagueManager.Enums.SessionType.Race || session.SessionType == iRLeagueManager.Enums.SessionType.HeatEvent)
            {
                var season = session.Schedule.Season;
                var seasonSessions = season.Schedules
                    .SelectMany(x => x.Sessions)
                    .Where(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race || x.SessionType == iRLeagueManager.Enums.SessionType.HeatEvent)
                    .OrderBy(x => x.Date);
                raceNr = (seasonSessions.Select((x, i) => new { number = i + 1, item = x }).FirstOrDefault(x => x.item.SessionId == sessionId)?.number).GetValueOrDefault();
            }

            // get scoredResults using ModelDataProvider
            var modelDataProvider = new ModelDataProvider(ProviderContext);
            ResultDataDTO rawResults = null;
            SimSessionDetailsDTO sessionDetails = null;
            if (session.SessionResult != null)
            {
                if (scoredResults == null)
                {
                    scoredResults = new ScoredResultDataDTO[0];
                    var resultSessions = new List<SessionBaseEntity>() { session };
                    if (session.SubSessions.Count > 0)
                    {
                        resultSessions.AddRange(session.SubSessions
                            .Where(x => x.SessionResult != null));
                    }
                    var ids = resultSessions
                        .SelectMany(x => x.Scorings
                            .Where(y => y.ShowResults == true)
                            .Select(y => new KeyValuePair<long, long>(y.ScoringId, x.SessionId)))
                        .ToArray();
                    //scoredResults = session.Scorings.Select(x => modelDataProvider.GetScoredResult(sessionId, x.ScoringId)).ToArray();
                    scoredResults = GetScoredResults(ids);
                }

                // get rawResults if includeRawResults == true
                if (includeRawResults)
                {
                    rawResults = mapper.MapToResulDataDTO(session.SessionResult);
                }

                // get session details
                var sessionDetailsData = session.SessionResult.IRSimSessionDetails;
                if (sessionDetailsData == null)
                {
                    sessionDetailsData = session.SubSessions?
                        .Select(x => x.SessionResult?.IRSimSessionDetails)
                        .FirstOrDefault(x => x != null);
                }
                sessionDetails = mapper.MapToSimSessionDetailsDTO(sessionDetailsData);
            }

            // construct SessionResultsDTO
            var resultsDTO = new SessionResultsDTO()
            {
                Count = scoredResults.Count(),
                ScheduleId = session.ScheduleId,
                ScheduleName = session.Schedule?.Name,
                RaceNr = raceNr,
                RawResults = rawResults,
                ScoredResults = scoredResults,
                SessionId = sessionId,
                SessionDetails = sessionDetails,
                LocationId = session.LocationId
            };

            return resultsDTO;
        }

        /// <summary>
        /// Get the results of a whole season
        /// </summary>
        /// <param name="seasonId">Id of the season</param>
        /// <param name="includeRawResults">If <see langword="true"/> raw result data is included</param>
        /// <returns>Convenience DTO for all season results</returns>
        public SeasonResultsDTO GetResultsFromSeason(long seasonId, bool includeRawResults = false)
        {
            var mapper = new DTOMapper(DbContext);

            // get season entity from database
            SeasonEntity season;
            if (seasonId == 0)
            {
                season = DbContext.Set<SeasonEntity>().OrderByDescending(x => x.SeasonStart).FirstOrDefault();
            }
            else
            {
                season = DbContext.Set<SeasonEntity>().Find(seasonId);
            }

            if (season == null)
            {
                return new SeasonResultsDTO() { SeasonId = seasonId };
            }

            // get season results
            var sessions = season.Schedules
                .SelectMany(x => x.Sessions)
                .Where(x => x.SessionResult != null);
            var resultSessions = sessions
                .Concat(sessions
                    .Where(x => x.SubSessions.Count > 0)
                    .SelectMany(x => x.SubSessions
                        .Where(y => y.SessionResult != null)));
            var ids = resultSessions
                .SelectMany(x => x.Scorings
                    .Where(y => y.ShowResults == true) 
                    .Select(y => new KeyValuePair<long, long>(y.ScoringId, x.SessionId)))
                .ToArray();
            var results = GetScoredResults(ids);
            var sessionResults = sessions
                .Select(x => GetResultsFromSession(x.SessionId, includeRawResults, results
                    .Where(y => x.SessionId == y.SessionId || x.SubSessions.Any(z => z.SessionId == y.SessionId)).ToArray()))
                .ToArray();

            // Construct DTO
            var seasonResults = new SeasonResultsDTO()
            {
                SeasonId = seasonId,
                SessionCount = sessions.Count(),
                SchedulesCount = season.Schedules.Count(),
                ResultsCount = sessions.Where(x => x.SessionResult != null).Count(),
                SessionResults = sessionResults
            };

            return seasonResults;
        }

        public ScoredResultDataDTO[] GetScoredResults(IEnumerable<KeyValuePair<long, long>> scoringSessionIds)
        {
            var scoredResultData = new ScoredResultDataDTO[0];
            var sessionIds = scoringSessionIds.Select(x => x.Value);
            var scoringIds = scoringSessionIds.Select(x => x.Key);

            DbContext.Configuration.LazyLoadingEnabled = false;

            /// Load results and check if recalculation needed
            var results = DbContext.Set<ResultEntity>().Where(x => sessionIds.Contains(x.ResultId))
                .Include(x => x.Session).ToList();

            foreach (var result in results)
            {
                if (result.RequiresRecalculation)
                {
                    ILeagueActionProvider leagueActionProvider = new LeagueActionProvider(DbContext);
                    leagueActionProvider.CalculateScoredResult(result.ResultId);
                }
            }

            IEnumerable<ScoredResultEntity> scoredResultEntities = DbContext.Set<ScoredResultEntity>()
                .Include(x => x.Scoring)
                .Where(x => sessionIds.Contains(x.ResultId))
                .ToArray(); // Filter data before fetching from database

            scoredResultEntities = scoringSessionIds
                .Select(x => scoredResultEntities
                    .SingleOrDefault(y => x.Key == y.ScoringId && x.Value == y.ResultId)).ToArray(); // Filter data after fetching from database to the exact needed scoredResults

            DbContext.Set<ScoredResultRowEntity>()
                .Where(x => sessionIds.Contains(x.ScoredResultId) && scoringIds.Contains(x.ScoringId))
                .Include(x => x.ResultRow)
                .Include(x => x.AddPenalty)
                .Include(x => x.ReviewPenalties)
                .Load();

            DbContext.Set<ScoredTeamResultRowEntity>()
                .Where(x => sessionIds.Contains(x.ScoredResultId) && scoringIds.Contains(x.ScoringId))
                .Include(x => x.ScoredResultRows)
                .Load();

            DbContext.Set<IncidentReviewEntity>()
                .Where(x => sessionIds.Contains(x.SessionId))
                .Include(x => x.AcceptedReviewVotes)
                .Load();

            DbContext.ChangeTracker.DetectChanges();

            var memberIds = new List<long>();
            foreach(var scoredResult in scoredResultEntities)
            {
                if (scoredResult.FinalResults != null)
                {
                    foreach (var row in scoredResult.FinalResults)
                    {
                        memberIds.Add(row.MemberId);
                    }
                }
                if (scoredResult is ScoredTeamResultEntity scoredTeamResultEntity && scoredTeamResultEntity.TeamResults != null)
                {
                    var scoredTeamResultRows = scoredTeamResultEntity.TeamResults
                        .SelectMany(x => x.ScoredResultRows)
                        .Where(x => x != null);
                    foreach(var row in scoredTeamResultRows)
                    {
                        memberIds.Add(row.MemberId);
                    }
                }
            }
            memberIds = memberIds.Distinct().ToList();

            DbContext.Set<LeagueMemberEntity>()
                .Where(x => memberIds.Contains(x.MemberId))
                .Load();

            //foreach (var scoredResultEntity in scoredResultEntities)
            //{
            //    if (scoredResultEntity is ScoredTeamResultEntity scoredTeamResultEntity)
            //    {
            //        DbContext.Entry(scoredTeamResultEntity).Collection(x => x.TeamResults).Query()
            //            .Include(x => x.ScoredResultRows).Load();
            //    }
            //}

            DbContext.ChangeTracker.DetectChanges();

            var mapper = new DTOMapper(DbContext);
            scoredResultData = scoredResultEntities.Select(x => mapper.MapTo<ScoredResultDataDTO>(x)).ToArray();
            DbContext.Configuration.LazyLoadingEnabled = true;

            return scoredResultData;
        }
    }
}