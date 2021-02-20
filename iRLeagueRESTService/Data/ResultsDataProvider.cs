using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Results.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace iRLeagueRESTService.Data
{
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
        public SessionResultsDTO GetResultsFromSession(long sessionId, bool includeRawResults = false)
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

            if (session == null || session.SessionResult == null)
            {
                return new SessionResultsDTO() { SessionId = sessionId };
            }
            sessionId = session.SessionId;

            // get session details
            var sessionDetails = mapper.MapToSimSessionDetailsDTO(session.SessionResult.IRSimSessionDetails);

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
            ScoredResultDataDTO[] scoredResults = session.Scorings.Select(x => modelDataProvider.GetScoredResult(sessionId, x.ScoringId)).ToArray();

            ResultDataDTO rawResults = null;
            // get rawResults if includeRawResults == true
            if (includeRawResults)
            {
                rawResults = mapper.MapToResulDataDTO(session.SessionResult);
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
            var sessionResults = sessions
                .Select(x => GetResultsFromSession(x.SessionId, includeRawResults))
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
    }
}