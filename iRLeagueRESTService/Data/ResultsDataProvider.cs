using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Mapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace iRLeagueRESTService.Data
{
    /// <summary>
    /// Data access class for retrieving review convenience data from League database
    /// </summary>
    public class ResultsDataProvider : DataProviderBase, IResultsDataProvider
    {
        public ResultsDataProvider(LeagueDbContext dbContext) : base(dbContext)
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
                    .Where(x => x.SessionResult != null)
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
            if (session.SessionType == iRLeagueManager.Enums.SessionType.Race)
            {
                var season = session.Schedule.Season;
                var seasonSessions = season.Schedules.SelectMany(x => x.Sessions).Where(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race).OrderBy(x => x.Date);
                raceNr = (seasonSessions.Select((x, i) => new { number = i + 1, item = x }).FirstOrDefault(x => x.item.SessionId == sessionId)?.number).GetValueOrDefault();
            }

            // get scoredResults using ModelDataProvider
            var modelDataProvider = new ModelDataProvider(DbContext);
            ResultDataDTO rawResults = null;
            SimSessionDetailsDTO sessionDetails = null;
            if (session.SessionResult != null)
            {
                if (scoredResults == null)
                {
                    scoredResults = new ScoredResultDataDTO[0];
                    var ids = session.Scorings.Select(x => new KeyValuePair<long, long>(x.ScoringId, session.SessionId));
                    //scoredResults = session.Scorings.Select(x => modelDataProvider.GetScoredResult(sessionId, x.ScoringId)).ToArray();
                    scoredResults = GetScoredResults(ids);
                }

                // get rawResults if includeRawResults == true
                if (includeRawResults)
                {
                    rawResults = mapper.MapToResulDataDTO(session.SessionResult);
                }

                // get session details
                sessionDetails = mapper.MapToSimSessionDetailsDTO(session.SessionResult.IRSimSessionDetails);
            }

            // construct SessionResultsDTO
            var resultsDTO = new SessionResultsDTO()
            {
                Count = scoredResults.Count(),
                ScheduleId = session.ScheduleId,
                ScheduleName = session.Schedule.Name,
                RaceNr = raceNr,
                RawResults = rawResults,
                ScoredResults = scoredResults,
                SessionId = sessionId,
                SessionDetails = sessionDetails
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
            var season = DbContext.Set<SeasonEntity>().Find(seasonId);

            if (season == null)
            {
                return new SeasonResultsDTO() { SeasonId = seasonId };
            }

            // get season results
            var sessions = season.Schedules.SelectMany(x => x.Sessions);
            var ids = sessions.SelectMany(x => x.Scorings.Select(y => new KeyValuePair<long, long>(y.ScoringId, x.SessionId))).ToArray();
            var results = GetScoredResults(ids);
            var sessionResults = sessions
                .Select(x => GetResultsFromSession(x.SessionId, includeRawResults, results.Where(y => x.SessionId == y.SessionId).ToArray()))
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
                .Include(x => x.Session);

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
                .Include(x => x.HardChargers)
                .Include(x => x.CleanestDrivers)
                .Where(x => sessionIds.Contains(x.ResultId))
                .ToArray(); // Filter data before fetching from database

            scoredResultEntities = scoringSessionIds
                .Select(x => scoredResultEntities
                    .SingleOrDefault(y => x.Key == y.ScoringId && x.Value == y.ResultId)).ToArray(); // Filter data after fetching from database to the exact needed scoredResults

            DbContext.Set<ScoredResultRowEntity>().Where(x => sessionIds.Contains(x.ScoredResultId) && scoringIds.Contains(x.ScoringId))
                     .Include(x => x.AddPenalty)
                     .Include(x => x.ResultRow.Member.Team)
                     .Include(x => x.ReviewPenalties).Load();

            DbContext.Set<IncidentReviewEntity>()
                     .Where(x => sessionIds.Contains(x.SessionId))
                     .Include(x => x.AcceptedReviewVotes)
                     .Load();

            foreach (var scoredResultEntity in scoredResultEntities)
            {
                if (scoredResultEntity is ScoredTeamResultEntity scoredTeamResultEntity)
                {
                    DbContext.Entry(scoredTeamResultEntity).Collection(x => x.TeamResults).Query()
                        .Include(x => x.ScoredResultRows).Load();
                }
            }

            DbContext.ChangeTracker.DetectChanges();

            var mapper = new DTOMapper(DbContext);
            scoredResultData = scoredResultEntities.Select(x => mapper.MapTo<ScoredResultDataDTO>(x)).ToArray();
            DbContext.Configuration.LazyLoadingEnabled = true;

            return scoredResultData;
        }
    }
}