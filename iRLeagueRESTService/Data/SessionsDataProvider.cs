using iRLeagueDatabase;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Sessions.Convenience;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Extensions;
using iRLeagueDatabase.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;

namespace iRLeagueRESTService.Data
{
    /// <summary>
    /// Data access class for retrieving review convenience data from League database
    /// </summary>
    public class SessionsDataProvider : DataProviderBase, ISessionsDataProvider
    {
        public SessionsDataProvider(LeagueDbContext dbContext) : base(dbContext)
        {
        }

        public SessionDataDTO GetSession(long sessionId, bool includeScheduleData = true)
        {
            var mapper = new DTOMapper(DbContext);

            // get session entity
            // if session id == 0 load latest session, else load specified session
            SessionBaseEntity session;
            if (sessionId == 0)
            {
                session = DbContext.Set<SessionBaseEntity>()
                    .Where(x => x.SessionResult != null && x.SessionType != iRLeagueManager.Enums.SessionType.Heat)
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
            }
            else
            {
                session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
            }

            if (session == null)
            {
                return null;
            }

            // get session race number
            int raceNr = 0;
            if (session.SessionType == iRLeagueManager.Enums.SessionType.Race)
            {
                var season = session.Schedule.Season;
                var seasonSessions = season.Schedules.SelectMany(x => x.Sessions).Where(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race).OrderBy(x => x.Date);
                raceNr = (seasonSessions.Select((x, i) => new { number = i + 1, item = x }).FirstOrDefault(x => x.item.SessionId == sessionId)?.number).GetValueOrDefault();
            }

            SessionDataDTO sessionDTO;
            if (session.SessionType == iRLeagueManager.Enums.SessionType.Race)
            {
                RaceSessionConvenienceDTO raceSessionDTO = (RaceSessionConvenienceDTO)mapper.MapToRaceSessionDataDTO(session as RaceSessionEntity, new RaceSessionConvenienceDTO());
                raceSessionDTO.RaceNr = raceNr;
                raceSessionDTO.ScheduleId = includeScheduleData ? session.ScheduleId : (long?)null;
                raceSessionDTO.ScheduleName = includeScheduleData ? session.Schedule.Name : null;
                raceSessionDTO.HasResult = session.SessionResult != null;
                sessionDTO = raceSessionDTO;
            }
            else
            {
                SessionConvenienceDTO sessionConvenienceDTO = (SessionConvenienceDTO)mapper.MapToSessionDataDTO(session, new SessionConvenienceDTO());
                sessionConvenienceDTO.ScheduleId = includeScheduleData ? session.ScheduleId : default;
                sessionConvenienceDTO.ScheduleName = includeScheduleData ? session.Schedule.Name : default;
                sessionConvenienceDTO.HasResult = session.SessionResult != null;
                sessionDTO = sessionConvenienceDTO;
            }

            return sessionDTO;
        }

        public ScheduleSessionsDTO GetSessionsFromSchedule(long scheduleId)
        {
            var mapper = new DTOMapper(DbContext);

            // get schedule entity
            var schedule = DbContext.Set<ScheduleEntity>().Find(scheduleId);

            if (schedule == null)
            {
                return new ScheduleSessionsDTO() { ScheduleId = scheduleId };
            }

            // get sessions data
            var sessions = schedule.Sessions.Select(x => GetSession(x.SessionId, includeScheduleData: false)).ToArray();

            // construct DTO
            var scheduleSessions = new ScheduleSessionsDTO()
            {
                ScheduleId = schedule.ScheduleId,
                ScheduleName = schedule.Name,
                Sessions = sessions,
                SessionsCount = sessions.Count(),
                SessionsFinished = sessions.Count(x => x.SessionResultId != null),
                RacesCount = sessions.Count(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race),
                RacesFinished = sessions.Count(x => x.SessionType == iRLeagueManager.Enums.SessionType.Race && x.SessionResultId != null)
            };

            return scheduleSessions;
        }

        public SeasonSessionsDTO GetSessionsFromSeason(long seasonId)
        {
            // get season entity from database
            SeasonEntity season;
            if (seasonId == 0)
            {
                // if season id is 0 get last (current) season
                season = DbContext.Set<SeasonEntity>().OrderByDescending(x => x.SeasonStart).FirstOrDefault();
            }
            else
            {
                // get season by id
                season = DbContext.Set<SeasonEntity>().Find(seasonId);
            }

            if (season == null)
            {
                return null;
            }

            // get schedules
            var schedules = season.Schedules.Select(x => GetSessionsFromSchedule(x.ScheduleId)).ToArray();

            // construct DTO
            var seasonSessions = new SeasonSessionsDTO()
            {
                Schedules = schedules,
                SeasonId = season.SeasonId,
                SessionsCount = schedules.Sum(x => x.SessionsCount),
                SessionsFinished = schedules.Sum(x => x.SessionsFinished),
                RacesCount = schedules.Sum(x => x.RacesCount),
                RacesFinished = schedules.Sum(x => x.RacesFinished),
                SeasonName = season.SeasonName
            };

            return seasonSessions;
        }
    }
}