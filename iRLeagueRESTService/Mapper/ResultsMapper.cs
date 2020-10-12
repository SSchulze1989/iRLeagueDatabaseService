using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;
using System.Data.Entity;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterResultsTypeMaps()
        {
            RegisterTypeMap<ResultEntity, ResultInfoDTO>(MapToResultInfoDTO);
            RegisterTypeMap<ResultEntity, ResultDataDTO>(MapToResulDataDTO);
            RegisterTypeMap<ResultRowEntity, ResultRowDataDTO>(MapToResultRowDataDTO);
            RegisterTypeMap<ScoredResultEntity, ScoredResultDataDTO>(MapToScoredResultDataDTO);
            RegisterTypeMap<ScoredResultRowEntity, ScoredResultRowDataDTO>(MapToScoredResultRowDataDTO);
            RegisterTypeMap<ScoringEntity, ScoringInfoDTO>(MapToScoringInfoDTO);
            RegisterTypeMap<ScoringEntity, ScoringDataDTO>(MapToScoringDataDTO);
            RegisterTypeMap<ScoringTableEntity, ScoringTableInfoDTO>(MapToScoringTableInfoDTO);
            RegisterTypeMap<ScoringTableEntity, ScoringTableDataDTO>(MapToScoringTableDataDTO);
            RegisterTypeMap<StandingsEntity, StandingsDataDTO>(MapToStandingsDataDTO);
            RegisterTypeMap<StandingsRowEntity, StandingsRowDataDTO>(MapToStandingsRowDataDTO);
            RegisterTypeMap<AddPenaltyEntity, AddPenaltyDTO>(MapToPenaltyDTO);
            RegisterTypeMap<ScoredTeamResultEntity, ScoredTeamResultDataDTO>(MapToScoredTeamResultDataDTO);
            RegisterTypeMap<ScoredTeamResultEntity, ScoredResultDataDTO>(src => new ScoredTeamResultDataDTO(), (src, trg) => MapToScoredTeamResultDataDTO(src, trg as ScoredTeamResultDataDTO), DefaultCompare);
            RegisterTypeMap<TeamStandingsEntity, TeamStandingsDataDTO>(MapToTeamStandingsDTO);
            RegisterTypeMap<TeamStandingsEntity, StandingsDataDTO>(src => new TeamStandingsDataDTO(), (src, trg) => MapToTeamStandingsDTO(src, trg as TeamStandingsDataDTO), DefaultCompare);
        }

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
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
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
            target.ResultId = source.ResultId;
            target.StartPosition = source.StartPosition;
            target.Status = source.Status;
            target.TeamName = source.Member.Team?.Name;
            target.LocationId = source.Result.Session.LocationId;
            target.Date = source.Date.GetValueOrDefault();
            target.OldIRating = source.OldIRating;
            target.NewIRating = source.NewIRating;

            return target;
        }

        public ScoredResultDataDTO MapToScoredResultDataDTO(ScoredResultEntity source, ScoredResultDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoredResultDataDTO();

            MapToResultInfoDTO(source.Result, target);
            target.Scoring = MapToScoringInfoDTO(source.Scoring);
            target.ScoringName = source.Scoring.Name;
            target.FinalResults = source.FinalResults?.Select(x => MapToScoredResultRowDataDTO(x)).OrderBy(x => x.FinalPosition).ToList();

            return target;
        }

        public ScoredResultRowDataDTO MapToScoredResultRowDataDTO(ScoredResultRowEntity source, ScoredResultRowDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoredResultRowDataDTO();

            MapToResultRowDataDTO(source.ResultRow, target);
            target.ScoredResultRowId = source.ScoredResultRowId;
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
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.DropWeeks = source.DropWeeks;
            target.IncPenaltyPoints = source.IncPenaltyPoints;
            target.Name = source.Name;
            target.Season = MapToSeasonInfoDTO(source.Season);
            target.Sessions = source.Sessions.Select(x => MapToSessionInfoDTO(x)).ToArray();
            target.Results = source.Results.Select(x => MapToResultInfoDTO(x)).ToArray();
            target.ConnectedSchedule = MapToScheduleInfoDTO(source.ConnectedSchedule);
            target.ScoringKind = source.ScoringKind;
            target.MaxResultsPerGroup = source.MaxResultsPerGroup;
            target.TakeGroupAverage = source.TakeGroupAverage;
            target.ExtScoringSource = MapToScoringInfoDTO(source.ExtScoringSource);
            target.TakeResultsFromExtSource = source.TakeResultsFromExtSource;

            return target;
        }

        public ScoringTableInfoDTO MapToScoringTableInfoDTO(ScoringTableEntity source, ScoringTableInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoringTableInfoDTO();

            MapToVersionInfoDTO(source, target);

            target.ScoringTableId = source.ScoringTableId;
            target.Name = source.Name;

            return target;
        }

        public ScoringTableDataDTO MapToScoringTableDataDTO(ScoringTableEntity source, ScoringTableDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoringTableDataDTO();

            MapToScoringTableInfoDTO(source, target);

            target.AverageRaceNr = source.AverageRaceNr;
            target.DropWeeks = source.DropWeeks;
            target.ScoringFactors = source.ScoringFactors;
            target.ScoringKind = source.ScoringKind;
            target.Scorings = source.Scorings.Select(x => MapToScoringInfoDTO(x)).ToArray();
            target.Season = MapToSeasonInfoDTO(source.Season);
            target.Sessions = source.Sessions.Select(x => MapToSessionInfoDTO(x)).ToArray();
            target.DropRacesOption = source.DropRacesOption;
            target.ResultsPerRaceCount = source.ResultsPerRaceCount;

            return target;
        }

        public StandingsDataDTO MapToStandingsDataDTO(StandingsEntity source, StandingsDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new StandingsDataDTO();

            target.CleanestDriver = MapToMemberInfoDTO(source.CleanestDriver);
            target.MostPenaltiesDriver = MapToMemberInfoDTO(source.MostPenaltiesDriver);
            target.MostPolesDriver = MapToMemberInfoDTO(source.MostPolesDriver);
            target.MostWinsDriver = MapToMemberInfoDTO(source.MostWinsDriver);
            target.Scoring = MapToScoringInfoDTO(source.Scoring);
            target.ScoringTableId = source.ScoringTable.ScoringTableId;
            target.StandingsRows = source.StandingsRows.Select(x => MapToStandingsRowDataDTO(x)).ToArray();

            return target;
        }

        public StandingsRowDataDTO MapToStandingsRowDataDTO(StandingsRowEntity source, StandingsRowDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new StandingsRowDataDTO();

            target.CarClass = source.CarClass;
            target.ClassId = source.ClassId;
            target.CompletedLaps = source.CompletedLaps;
            target.CompletedLapsChange = source.CompletedLapsChange;
            target.DroppedResults = source.DroppedResults;
            target.FastestLaps = source.FastestLaps;
            target.FastestLapsChange = source.FastestLapsChange;
            target.Incidents = source.Incidents;
            target.IncidentsChange = source.IncidentsChange;
            target.LastPosition = source.LastPosition;
            target.LeadLaps = source.LeadLaps;
            target.LeadLapsChange = source.LeadLapsChange;
            target.Member = MapToMemberInfoDTO(source.Member);
            target.PenaltyPoints = source.PenaltyPoints;
            target.PenaltyPointsChange = source.PenaltyPointsChange;
            target.PolePositions = source.PolePositions;
            target.PolePositionsChange = source.PolePositionsChange;
            target.Position = source.Position;
            target.PositionChange = source.PositionChange;
            target.RacePoints = source.RacePoints;
            target.RacePointsChange = source.RacePointsChange;
            target.Races = source.Races;
            target.RacesCounted = source.RacesCounted;
            target.Scoring = MapToScoringInfoDTO(source.Scoring);
            target.Top10 = source.Top10;
            target.Top3 = source.Top3;
            target.Top5 = source.Top5;
            target.TotalPoints = source.TotalPoints;
            target.TotalPointsChange = source.TotalPointsChange;
            target.Wins = source.Wins;
            target.WinsChange = source.WinsChange;
            target.CountedResults = source.CountedResults?.Select(x => MapToScoredResultRowDataDTO(x)).ToArray();

            return target;
        }

        public AddPenaltyDTO MapToPenaltyDTO(AddPenaltyEntity source, AddPenaltyDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new AddPenaltyDTO();

            target.ScoredResultRowId = source.ScoredResultRowId;
            target.PenaltyPoints = source.PenaltyPoints;

            return target;
        }

        public ScoredTeamResultDataDTO MapToScoredTeamResultDataDTO(ScoredTeamResultEntity source, ScoredTeamResultDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoredTeamResultDataDTO();

            MapToScoredResultDataDTO(source, target);
            target.TeamResults = source.TeamResults != null ? source.TeamResults.Select(x => MapToScoredTeamResultRowDataDTO(x)).ToArray() : new ScoredTeamResultRowDataDTO[0];

            return target;
        }

        public ScoredTeamResultRowDataDTO MapToScoredTeamResultRowDataDTO(ScoredTeamResultRowEntity source, ScoredTeamResultRowDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoredTeamResultRowDataDTO();

            //MapToResultRowDataDTO(source.ResultRow, target);
            target.ScoredResultRowId = source.ScoredResultRowId;
            target.BonusPoints = source.BonusPoints;
            target.FinalPosition = source.FinalPosition;
            target.FinalPositionChange = source.FinalPositionChange;
            target.PenaltyPoints = source.PenaltyPoints;
            target.RacePoints = source.RacePoints;
            target.ScoringId = source.ScoringId;
            target.TeamId = source.TeamId;
            target.FastestLapTime = TimeSpanConverter.Convert(source.FastestLapTime);
            target.AvgLapTime = TimeSpanConverter.Convert(source.AvgLapTime);
            target.ScoredResultRows = source.ScoredResultRows.Select(x => MapToScoredResultRowDataDTO(x)).ToArray();

            return target;
        }

        public TeamStandingsDataDTO MapToTeamStandingsDTO(TeamStandingsEntity source, TeamStandingsDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new TeamStandingsDataDTO();

            MapToStandingsDataDTO(source, target);
            var teamStandingsRows = source.StandingsRows.OfType<TeamStandingsRowEntity>();
            target.StandingsRows = teamStandingsRows.Select(x => MapToTeamStandingsRowDTO(x)).ToArray();

            return target;
        }

        public TeamStandingsRowDataDTO MapToTeamStandingsRowDTO(TeamStandingsRowEntity source, TeamStandingsRowDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new TeamStandingsRowDataDTO();

            MapToStandingsRowDataDTO(source, target);

            target.TeamId = (source.Team?.TeamId).GetValueOrDefault();
            target.DriverStandingsRows = source.DriverStandingsRows.Select(x => MapToStandingsRowDataDTO(x)).ToArray();

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
            RegisterTypeMap<AddPenaltyDTO, AddPenaltyEntity>(MapToPenaltyEntity);
            RegisterTypeMap<ScoringTableDataDTO, ScoringTableEntity>(MapToScoringTableEntity);
        }

        public ResultEntity GetResultEntity(ResultInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //ResultEntity target;

            //if (source.ResultId == null)
            //    target = new ResultEntity();
            //else
            //    target = DbContext.Set<ResultEntity>().Find(source.ResultId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(ResultEntity), "Could not find Entity in Database.", source.ResultId);

            //return target;
            return DefaultGet<ResultInfoDTO, ResultEntity>(source);
        }

        public ResultEntity MapToResultEntity(ResultDataDTO source, ResultEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetResultEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            MapCollection(source.RawResults, target.RawResults, MapToResultRowEntity, x => new object[] { x.ResultRowId, x.ResultId });
            MapCollection(source.Reviews, target.Reviews, GetReviewEntity, x => x.ReviewId);
            target.Session = GetSessionBaseEntity(source.Session);
            target.Season = target.Session.Schedule.Season;

            return target;
        }

        public ResultRowEntity GetResultRowEntity(ResultRowDataDTO source)
        {
            //if (source == null)
            //    return null;
            //ResultRowEntity target;

            //if (source.ResultRowId == null)
            //    target = new ResultRowEntity();
            //else
            //    target = DbContext.Set<ResultRowEntity>().Find(source.ResultRowId, source.ResultId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(ResultRowEntity), "Could not find Entity in Database.", source.ResultRowId, source.ResultId);

            //return target;
            return DefaultGet<ResultRowDataDTO, ResultRowEntity>(source);
        }

        public ScoredResultRowEntity GetScoredResultRowEntity(ScoredResultRowDataDTO source)
        {
            //if (source == null)
            //    return null;
            //ScoredResultRowEntity target;

            //if (source.ResultRowId == null)
            //    target = new ScoredResultRowEntity();
            //else
            //    target = DbContext.Set<ScoredResultRowEntity>().Find(source.ResultRowId, source.ResultId, source.ScoringId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(ScoredResultRowEntity), "Could not find Entity in Database.", source.ResultRowId, source.ResultId, source.ScoringId);

            //return target;
            return DefaultGet<ScoredResultRowDataDTO, ScoredResultRowEntity>(source);
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
            target.Result = GetResultEntity(new ResultInfoDTO() { ResultId = source.ResultId });
            target.OldIRating = source.OldIRating;
            target.NewIRating = source.NewIRating;

            //compare with other resultrows in this season and determine SeasonStartIRating
            DbContext.Configuration.LazyLoadingEnabled = false;
            DbContext.Set<ResultEntity>()
                .Where(x => x.SeasonId == target.Result.SeasonId)
                .Include(x => x.RawResults)
                .Load();
            var seasonResultRows = DbContext.Set<ResultRowEntity>().Local;

            if (seasonResultRows.Any(x => x.MemberId == target.MemberId))
            {
                target.SeasonStartIRating = seasonResultRows.First(x => x.MemberId == target.MemberId).SeasonStartIRating;
            }
            else
            {
                target.SeasonStartIRating = target.OldIRating;
            }
            DbContext.Configuration.LazyLoadingEnabled = true;

            return target;
        }

        public ScoringEntity GetScoringEntity(ScoringInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //ScoringEntity target;

            //if (source.ScoringId == null)
            //    target = new ScoringEntity();
            //else
            //    target = DbContext.Set<ScoringEntity>().Find(source.ScoringId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(ScoringEntity), "Could not find Entity in Database.", source.ScoringId);

            //return target;
            return DefaultGet<ScoringInfoDTO, ScoringEntity>(source);
        }

        public ScoringEntity MapToScoringEntity(ScoringDataDTO source, ScoringEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                GetScoringEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.AverageRaceNr = source.AverageRaceNr;
            target.BasePoints = source.BasePoints;
            target.BonusPoints = source.BonusPoints;
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.DropWeeks = source.DropWeeks;
            target.IncPenaltyPoints = source.IncPenaltyPoints;
            target.Name = source.Name;
            target.Season = GetSeasonEntity(source.Season);
            if (target.Sessions == null)
                target.Sessions = new List<Entities.Sessions.SessionBaseEntity>();
            else
                MapCollection(source.Sessions, target.Sessions, GetSessionBaseEntity, x => x.SessionId);
            target.ConnectedSchedule = GetScheduleEntity(source.ConnectedSchedule);
            if (target.ConnectedSchedule != null)
            {
                target.Sessions = target.ConnectedSchedule.Sessions;
            }
            target.ScoringKind = source.ScoringKind;
            target.MaxResultsPerGroup = source.MaxResultsPerGroup;
            target.TakeGroupAverage = source.TakeGroupAverage;
            target.ExtScoringSource = GetScoringEntity(source.ExtScoringSource);
            target.TakeResultsFromExtSource = source.TakeResultsFromExtSource;

            return target;
        }

        public ScoringTableEntity GetScoringTableEntity(ScoringTableInfoDTO source)
        {
            return DefaultGet<ScoringTableInfoDTO, ScoringTableEntity>(source);
        }

        public ScoringTableEntity MapToScoringTableEntity(ScoringTableDataDTO source, ScoringTableEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetScoringTableEntity(source);

            if (MapToRevision(source, target) == false)
                return target;

            target.DropWeeks = source.DropWeeks;
            target.Name = source.Name;
            target.ScoringKind = source.ScoringKind;
            target.AverageRaceNr = source.AverageRaceNr;
            target.ScoringFactors = source.ScoringFactors;
            if (target.Scorings == null)
                target.Scorings = new List<ScoringEntity>();
            MapCollection(source.Scorings, target.Scorings, GetScoringEntity, x => x.Keys, removeFromCollection: true);
            target.ResultsPerRaceCount = source.ResultsPerRaceCount;
            target.DropRacesOption = source.DropRacesOption;

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

        public AddPenaltyEntity GetPenaltyEntity(AddPenaltyDTO source)
        {
            if (source == null)
                return null;

            return DefaultGet<AddPenaltyDTO, AddPenaltyEntity>(source);
        }

        public AddPenaltyEntity MapToPenaltyEntity(AddPenaltyDTO source, AddPenaltyEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetPenaltyEntity(source);

            target.ScoredResultRow = GetScoredResultRowEntity(new ScoredResultRowDataDTO() { ScoredResultRowId = source.ScoredResultRowId });
            target.PenaltyPoints = source.PenaltyPoints;

            return target;
        }
    }
}
