using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public ResultInfoDTO MapToResultInfoDTO(ResultEntity source, ResultInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ResultInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.ResultId = source.ResultId;
            target.Session = MapToSessionInfoDTO(source.Session);

            return target;
        }

        public ResultDataDTO MapToResulDataDTO(ResultEntity source, ResultDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ResultDataDTO();

            MapToResultInfoDTO(source, target);
            target.CreatedBy = MapToMemberInfoDTO(source.CreatedBy);
            target.LastModifiedBy = MapToMemberInfoDTO(source.LastModifiedBy);
            target.RawResults = source.RawResults.Select(x => MapToResultRowDataDTO(x)).ToList();
            target.ResultId = source.ResultId;
            target.Reviews = source.Reviews.Select(x => MapToReviewInfoDTO(x));
            target.Season = MapToSeasonInfoDTO(source.Session.Schedule.Season);
            target.Session = MapToSessionInfoDTO(source.Session);

            return target;
        }

        public ResultRowDataDTO MapToResultRowDataDTO(ResultRowEntity source, ResultRowDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ResultRowDataDTO();

            target.AvgLapTime = TimeSpanConverter.Convert(source.AvgLapTime);
            target.Car = source.Car;
            target.CarClass = source.CarClass;
            target.CarNumber = source.CarNumber;
            target.ClassId = source.ClassId;
            target.CompletedLaps = source.CompletedLaps;
            target.FastestLapTime = TimeSpanConverter.Convert(source.FastestLapTime);
            target.FastLapNr = source.FastLapNr;
            target.FinishPosition = source.FinishPosition;
            target.Incidents = source.Incidents;
            target.Interval = TimeSpanConverter.Convert(source.Interval);
            target.LeadLaps = source.LeadLaps;
            target.Member = MapToMemberInfoDTO(source.Member);
            target.PositionChange = source.PositionChange;
            target.QualifyingTime = TimeSpanConverter.Convert(source.QualifyingTime);
            target.ResultRowId = source.ResultRowId;
            target.StartPosition = source.StartPosition;
            target.Status = source.Status;

            return target;
        }

        public ScoredResultRowDataDTO MapToScoredResultRowDataDTO(ScoredResultRowEntity source, ScoredResultRowDataDTO target)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoredResultRowDataDTO();

            MapToResultRowDataDTO(source.ResultRow, target);
            target.BonusPoints = source.BonusPoints;
            target.FinalPosition = source.FinalPosition;
            target.FinalPositionChange = source.FinalPositionChange;
            target.PenaltyPoints = source.PenaltyPoints;
            target.RacePoints = source.RacePoints;
            target.ScoringId = source.ScoringId;

            return target;
        }

        public ScoringInfoDTO MapToScoringInfoDTO(ScoringEntity source, ScoringInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoringInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.ScoringId = source.ScoringId;

            return target;
        }

        public ScoringDataDTO MapToScoringDataDTO(ScoringEntity source, ScoringDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoringDataDTO();

            MapToScoringInfoDTO(source, target);
            target.AverageRaceNr = source.AverageRaceNr;
            target.BasePoints = source.BasePoints;
            target.BonusPoints = source.BonusPoints;
            target.CreatedBy = MapToMemberInfoDTO(source.CreatedBy);
            target.DropWeeks = source.DropWeeks;
            target.IncPenaltyPoints = source.IncPenaltyPoints;
            target.LastModifiedBy = MapToMemberInfoDTO(source.LastModifiedBy);
            target.MultiScoringFactors = source.MultiScoringFactors;
            target.MultiScoringResults = source.MultiScoringResults.Select(x => MapToScoringInfoDTO(x)).ToList();
            target.Name = source.Name;
            target.Season = MapToSeasonInfoDTO(source.Season);
            target.SeasonId = source.SeasonId;
            target.Sessions = source.Sessions.Select(x => MapToSessionInfoDTO(x)).ToList();

            return target;
        }
    }

    public partial class EntityMapper
    {
        private void RegisterResultsTypeMaps()
        {
            RegisterTypeMap<ResultDataDTO, ResultEntity>(MapToResultEntity);
            RegisterTypeMap<ScoringDataDTO, ScoringEntity>(MapToScoringEntity);
            RegisterTypeMap<ResultRowDataDTO, ResultRowEntity>(MapToResultRowEntity);
        }

        public ResultEntity GetResultEntity(ResultInfoDTO source)
        {
            if (source == null)
                return null;
            ResultEntity target;

            if (source.ResultId == null)
                target = new ResultEntity();
            else
                target = DbContext.Set<ResultEntity>().Find(source.ResultId);
            
            if (target == null)
                throw new EntityNotFoundException(nameof(ResultEntity), "Could not find Entity in Database.", source.ResultId);

            return target;
        }

        public ResultEntity MapToResultEntity(ResultDataDTO source, ResultEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetResultEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.CreatedBy = GetMemberEntity(source.CreatedBy);
            target.LastModifiedBy = GetMemberEntity(source.LastModifiedBy);
            MapCollection(source.RawResults, target.RawResults, MapToResultRowEntity, x => new object[] { x.ResultRowId, x.ResultId });
            MapCollection(source.Reviews, target.Reviews, GetReviewEntity, x => x.ReviewId);
            target.Session = GetSessionBaseEntity(source.Session);

            return target;
        }

        public ResultRowEntity GetResultRowEntity(ResultRowDataDTO source)
        {
            if (source == null)
                return null;
            ResultRowEntity target;

            if (source.ResultRowId == null)
                target = new ResultRowEntity();
            else
                target = DbContext.Set<ResultRowEntity>().Find(source.ResultRowId, source.ResultId);

            if (target == null)
                throw new EntityNotFoundException(nameof(ResultRowEntity), "Could not find Entity in Database.", source.ResultRowId, source.ResultId);

            return target;
        }

        public ScoredResultRowEntity GetScoredResultRowEntity(ScoredResultRowDataDTO source)
        {
            if (source == null)
                return null;
            ScoredResultRowEntity target;

            if (source.ResultRowId == null)
                target = new ScoredResultRowEntity();
            else
                target = DbContext.Set<ScoredResultRowEntity>().Find(source.ResultRowId, source.ResultId, source.ScoringId);

            if (target == null)
                throw new EntityNotFoundException(nameof(ScoredResultRowEntity), "Could not find Entity in Database.", source.ResultRowId, source.ResultId, source.ScoringId);

            return target;
        }

        public ResultRowEntity MapToResultRowEntity(ResultRowDataDTO source, ResultRowEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetResultRowEntity(source);

            target.AvgLapTime = TimeSpanConverter.Convert(source.AvgLapTime);
            target.Car = source.Car;
            target.CarClass = source.CarClass;
            target.CarNumber = source.CarNumber;
            target.ClassId = source.ClassId;
            target.CompletedLaps = source.CompletedLaps;
            target.FastestLapTime = TimeSpanConverter.Convert(source.FastestLapTime);
            target.FastLapNr = source.FastLapNr;
            target.FinishPosition = source.FinishPosition;
            target.Incidents = source.Incidents;
            target.Interval = TimeSpanConverter.Convert(source.Interval);
            target.LeadLaps = source.LeadLaps;
            target.Member = GetMemberEntity(source.Member);
            target.PositionChange = source.PositionChange;
            target.QualifyingTime = TimeSpanConverter.Convert(source.QualifyingTime);
            target.StartPosition = source.StartPosition;
            target.Status = source.Status;

            return target;
        }

        public ScoringEntity GetScoringEntity(ScoringInfoDTO source)
        {
            if (source == null)
                return null;
            ScoringEntity target;

            if (source.ScoringId == null)
                target = new ScoringEntity();
            else
                target = DbContext.Set<ScoringEntity>().Find(source.ScoringId);

            if (target == null)
                throw new EntityNotFoundException(nameof(ScoringEntity), "Could not find Entity in Database.", source.ScoringId);

            return target;
        }

        public ScoringEntity MapToScoringEntity(ScoringDataDTO source, ScoringEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                GetScoringEntity(source);

            if (!MapToRevision(source, target))
                return target;

            //target.ScoringId = source.ScoringId.GetValueOrDefault();
            target.AverageRaceNr = source.AverageRaceNr;
            target.BasePoints = source.BasePoints;
            target.BonusPoints = source.BonusPoints;
            target.CreatedBy = GetMemberEntity(source.CreatedBy);
            target.DropWeeks = source.DropWeeks;
            target.IncPenaltyPoints = source.IncPenaltyPoints;
            target.LastModifiedBy = GetMemberEntity(source.LastModifiedBy);
            target.MultiScoringFactors = source.MultiScoringFactors;
            MapCollection(source.MultiScoringResults, target.MultiScoringResults, GetScoringEntity, x => x.ScoringId);
            target.Name = source.Name;
            target.Season = GetSeasonEntity(source.Season);
            MapCollection(source.Sessions, target.Sessions, GetSessionBaseEntity, x => x.SessionId);

            return target;
        }

        public ScoredResultRowEntity MapToScoredResultRowEntity(ScoredResultRowDataDTO source, ScoredResultRowEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetScoredResultRowEntity(source);

            target.ResultRow = GetResultRowEntity(source);
            target.BonusPoints = source.BonusPoints;
            target.FinalPosition = source.FinalPosition;
            target.FinalPositionChange = source.FinalPositionChange;
            target.PenaltyPoints = source.PenaltyPoints;
            target.RacePoints = source.RacePoints;

            return target;
        }
    }
}
