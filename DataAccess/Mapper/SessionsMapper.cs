using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using System.Collections.Generic;
using System.Linq;

namespace iRLeagueDatabase.DataAccess.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterSessionsTypeMaps()
        {
            RegisterTypeMap<ScheduleEntity, ScheduleInfoDTO>(MapToScheduleInfoDTO);
            RegisterTypeMap<ScheduleEntity, ScheduleDataDTO>(MapToScheduleDataDTO);
            RegisterTypeMap<SessionBaseEntity, SessionInfoDTO>(MapToSessionInfoDTO);
            RegisterTypeMap<SessionBaseEntity, SessionDataDTO>(MapToSessionDataDTO);
            RegisterTypeMap<RaceSessionEntity, RaceSessionDataDTO>(MapToRaceSessionDataDTO);
            RegisterTypeMap<RaceSessionEntity, SessionInfoDTO>(src => new SessionInfoDTO(), MapToSessionInfoDTO, DefaultCompare);
            RegisterTypeMap<RaceSessionEntity, SessionDataDTO>(src => new RaceSessionDataDTO(), (src, trg) => MapToRaceSessionDataDTO(src, trg as RaceSessionDataDTO), DefaultCompare);
        }

        public SessionInfoDTO MapToSessionInfoDTO(SessionBaseEntity source, SessionInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new SessionInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.SessionId = source.SessionId;
            target.SessionType = source.SessionType;
            target.Name = source.Name;

            return target;
        }

        public SessionDataDTO MapToSessionDataDTO(SessionBaseEntity source, SessionDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new SessionDataDTO();

            MapToSessionInfoDTO(source, target);
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.Date = source.Date.GetValueOrDefault();
            target.Duration = source.Duration;
            target.LocationId = source.LocationId;
            target.ScheduleId = source.ScheduleId; // MapToScheduleInfoDTO(source.Schedule);
            target.SessionResultId = source.SessionResult?.ResultId; // MapToResultInfoDTO(source.SessionResult);
            target.ReviewIds = source.Reviews.Select(x => x.ReviewId).ToArray();
            target.SubSessions = source.SubSessions.Select(x => MapToSessionDataDTO(x)).ToArray();
            target.ParentSessionId = source.ParentSession?.SessionId;
            target.SubSessionNr = source.SubSessionNr;

            return target;
        }

        public RaceSessionDataDTO MapToRaceSessionDataDTO(RaceSessionEntity source, RaceSessionDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new RaceSessionDataDTO();

            MapToSessionDataDTO(source, target);
            target.IrResultLink = source.IrResultLink;
            target.IrSessionId = source.IrSessionId;
            target.Laps = source.Laps;
            target.PracticeAttached = source.PracticeAttached;
            target.PracticeLength = source.PracticeLength;
            target.QualyAttached = source.QualyAttached;
            target.QualyLength = source.QualyLength;
            //target.RaceId = source.RaceId;
            target.RaceLength = source.RaceLength;

            return target;
        }

        public ScheduleInfoDTO MapToScheduleInfoDTO(ScheduleEntity source, ScheduleInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScheduleInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.ScheduleId = source.ScheduleId;
            target.Name = source.Name;

            return target;
        }

        public ICollection<SessionDataDTO> MapSessionDataDTOCollection(IEnumerable<SessionBaseEntity> source)
        {
            ICollection<SessionDataDTO> target = source.Select(sourceItem =>
            {
                SessionDataDTO targetItem;

                if (sourceItem is RaceSessionEntity sourceRace)
                {
                    targetItem = MapToRaceSessionDataDTO(sourceRace);
                }
                else
                {
                    targetItem = MapToSessionDataDTO(sourceItem);
                }

                return targetItem;
            }).ToList();

            return target;
        }

        public ScheduleDataDTO MapToScheduleDataDTO(ScheduleEntity source, ScheduleDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScheduleDataDTO();

            MapToScheduleInfoDTO(source, target);
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.Name = source.Name;
            target.Sessions = source.Sessions.Select(MapTo<SessionDataDTO>).ToArray();

            return target;
        }
    }

    public partial class EntityMapper
    {
        public void RegisterSessionsTypeMaps()
        {
            RegisterTypeMap<SessionDataDTO, SessionBaseEntity>(src =>
            {
                if (src is RaceSessionDataDTO raceSession)
                {
                    return GetRaceSessionEntity(raceSession);
                }
                return GetSessionBaseEntity(src);
            }, (src, trg) =>
            {
                if (src is RaceSessionDataDTO raceSession)
                    return MapToRaceSessionEntity(raceSession);
                return MapToSessionBaseEntity(src);
            }, DefaultCompare);
            RegisterTypeMap<RaceSessionDataDTO, RaceSessionEntity>(MapToRaceSessionEntity);
            RegisterTypeMap<RaceSessionDataDTO, SessionBaseEntity>(GetRaceSessionEntity, (src, trg) => MapToRaceSessionEntity(src, trg as RaceSessionEntity), DefaultCompare);
            RegisterTypeMap<ScheduleDataDTO, ScheduleEntity>(MapToScheduleEntity);
        }

        public SessionBaseEntity GetSessionBaseEntity(SessionInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //SessionBaseEntity target;

            //if (source is RaceSessionDataDTO raceSession)
            //    target = GetRaceSessionEntity(raceSession);
            //else if (source.SessionId == null)
            //    target = new SessionBaseEntity();
            //else
            //    target = DbContext.Set<SessionBaseEntity>().Find(source.SessionId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(SessionBaseEntity), "Could not find Entity in Database.", source.SessionId);

            //return target;
            return DefaultGet<SessionInfoDTO, SessionBaseEntity>(source);
        }

        public RaceSessionEntity GetRaceSessionEntity(RaceSessionDataDTO source)
        {
            //if (source == null)
            //    return null;
            //RaceSessionEntity target;

            //if (source.SessionId == null)
            //    target = new RaceSessionEntity();
            //else
            //    target = DbContext.Set<RaceSessionEntity>().Find(source.SessionId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(RaceSessionEntity), "Could not find Entity in Database.", source.SessionId);

            //return target;
            return DefaultGet<RaceSessionDataDTO, RaceSessionEntity>(source);
        }

        public SessionBaseEntity MapToSessionBaseEntity(SessionDataDTO source, SessionBaseEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetSessionBaseEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.SessionType = source.SessionType;
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.Date = source.Date;
            target.Duration = source.Duration;
            target.LocationId = source.LocationId;
            //target.Schedule = GetScheduleEntity(source.Schedule);
            //target.SessionResult = GetResultEntity(new DataTransfer.Results.ResultInfoDTO() { ResultId = source.SessionResultId });
            target.SessionResult = DefaultGet<ResultEntity>(source.SessionId);
            target.Name = source.Name;
            target.SubSessionNr = source.SubSessionNr;
            if (target.SubSessions == null)
            {
                target.SubSessions = new List<SessionBaseEntity>();
            }
            MapCollection(source.SubSessions, target.SubSessions, MapToSessionBaseEntity, x => x.SessionId, removeFromCollection: true, removeFromDatabase: true, autoAddMissing: true);
            //MapCollection(source.Reviews, target.Reviews, (src, trg) => GetReviewEntity(src), src => src.ReviewId);

            return target;
        }

        public RaceSessionEntity MapToRaceSessionEntity(RaceSessionDataDTO source, RaceSessionEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetRaceSessionEntity(source);

            if (!MapToRevision(source, target))
                return target;

            MapToSessionBaseEntity(source, target);
            target.IrResultLink = source.IrResultLink;
            target.IrSessionId = source.IrSessionId;
            target.Laps = source.Laps;
            target.PracticeAttached = source.PracticeAttached;
            target.PracticeLength = source.PracticeLength;
            target.QualyAttached = source.QualyAttached;
            target.QualyLength = source.QualyLength;
            //target.RaceId = source.RaceId;
            target.RaceLength = source.RaceLength;

            return target;
        }

        public ScheduleEntity GetScheduleEntity(ScheduleInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //ScheduleEntity target;

            //if (source.ScheduleId == null)
            //    target = new ScheduleEntity();
            //else
            //    target = DbContext.Set<ScheduleEntity>().Find(source.ScheduleId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(ScheduleEntity), "Could not find Entity in Database.", source.ScheduleId);

            //return target;
            return DefaultGet<ScheduleInfoDTO, ScheduleEntity>(source);
        }

        public ScheduleEntity MapToScheduleEntity(ScheduleDataDTO source, ScheduleEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                GetScheduleEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.Name = source.Name;
            MapCollection(source.Sessions, target.Sessions, (src, trg) =>
            {
                if (src is RaceSessionDataDTO raceSession)
                    return MapToRaceSessionEntity(raceSession, trg as RaceSessionEntity);

                return MapToSessionBaseEntity(src, trg);
            }, x => x.SessionId, autoAddMissing: true);

            return target;
        }
    }
}
