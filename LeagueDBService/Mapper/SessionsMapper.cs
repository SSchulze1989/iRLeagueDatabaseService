using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.DataTransfer.Sessions;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public SessionInfoDTO MapToSessionInfoDTO(SessionBaseEntity source, SessionInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new SessionInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.SessionId = source.SessionId;
            target.SessionType = source.SessionType;

            return target;
        }

        public SessionDataDTO MapToSessionDataDTO(SessionBaseEntity source, SessionDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new SessionDataDTO();

            MapToSessionInfoDTO(source, target);
            target.CreatedBy = MapToMemberInfoDTO(source.CreatedBy);
            target.Date = source.Date.GetValueOrDefault();
            target.Duration = source.Duration;
            target.LastModifiedBy = MapToMemberInfoDTO(source.LastModifiedBy);
            target.LocationId = source.LocationId;
            target.Schedule = MapToScheduleInfoDTO(source.Schedule);
            target.SessionResult = MapToResultInfoDTO(source.SessionResult);

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
            target.RaceId = source.RaceId;
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
            target.CreatedBy = MapToMemberInfoDTO(source.CreatedBy);
            target.LastModifiedBy = MapToMemberInfoDTO(source.LastModifiedBy);
            target.Name = source.Name;
            target.Sessions =  MapSessionDataDTOCollection(source.Sessions);

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
        }

        public SessionBaseEntity GetSessionBaseEntity(SessionInfoDTO source)
        {
            if (source == null)
                return null;
            SessionBaseEntity target;

            if (source is RaceSessionDataDTO raceSession)
                target = GetRaceSessionEntity(raceSession);
            else if (source.SessionId == null)
                target = new SessionBaseEntity();
            else
                target = DbContext.Set<SessionBaseEntity>().Find(source.SessionId);

            if (target == null)
                throw new EntityNotFoundException(nameof(SessionBaseEntity), "Could not find Entity in Database.", source.SessionId);

            return target;
        }

        public RaceSessionEntity GetRaceSessionEntity(RaceSessionDataDTO source)
        {
            if (source == null)
                return null;
            RaceSessionEntity target;

            if (source.SessionId == null)
                target = new RaceSessionEntity();
            else
                target = DbContext.Set<RaceSessionEntity>().Find(source.SessionId);

            if (target == null)
                throw new EntityNotFoundException(nameof(RaceSessionEntity), "Could not find Entity in Database.", source.SessionId);

            return target;
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
            target.CreatedBy = GetMemberEntity(source.CreatedBy);
            target.Date = source.Date;
            target.Duration = source.Duration;
            target.LastModifiedBy = GetMemberEntity(source.LastModifiedBy);
            target.LocationId = source.LocationId;
            //target.Schedule = GetScheduleEntity(source.Schedule);
            target.SessionResult = GetResultEntity(source.SessionResult);

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
            target.RaceId = source.RaceId;
            target.RaceLength = source.RaceLength;

            return target;
        }

        public ScheduleEntity GetScheduleEntity(ScheduleInfoDTO source)
        {
            if (source == null)
                return null;
            ScheduleEntity target;

            if (source.ScheduleId == null)
                target = new ScheduleEntity();
            else
                target = DbContext.Set<ScheduleEntity>().Find(source.ScheduleId);
            
            if (target == null)
                throw new EntityNotFoundException(nameof(ScheduleEntity), "Could not find Entity in Database.", source.ScheduleId);

            return target;
        }

        public ScheduleEntity MapToScheduleEntity(ScheduleDataDTO source, ScheduleEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                GetScheduleEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.CreatedBy = GetMemberEntity(source.CreatedBy);
            target.LastModifiedBy = GetMemberEntity(source.LastModifiedBy);
            target.Name = source.Name;
            MapCollection(source.Sessions, target.Sessions, (src, trg) =>
            {
                if (src is RaceSessionDataDTO raceSession)
                    return MapToRaceSessionEntity(raceSession, trg as RaceSessionEntity);

                return MapToSessionBaseEntity(src, trg);
            }, x => x.SessionId);

            return target;
        }
    }
}
