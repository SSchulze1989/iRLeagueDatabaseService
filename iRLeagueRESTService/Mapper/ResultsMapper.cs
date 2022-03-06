using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Extensions;
using iRLeagueManager.Timing;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
            RegisterTypeMap<IRSimSessionDetailsEntity, SimSessionDetailsDTO>(MapToSimSessionDetailsDTO);
        }

        public ResultInfoDTO MapToResultInfoDTO(ResultEntity source, ResultInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ResultInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.ResultId = source.ResultId;
            target.SessionId = source.Session.SessionId;//MapToSessionInfoDTO(source.Session);

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
            target.RawResults = source.RawResults.Select(x => MapToResultRowDataDTO(x)).ToArray();
            target.ResultId = source.ResultId;
            target.ReviewIds = source.Reviews.Select(x => x.ReviewId).ToArray();//source.Reviews.Select(x => MapToReviewInfoDTO(x)).ToArray();
            target.SeasonId = source.SeasonId.GetValueOrDefault();//MapToSeasonInfoDTO(source.Session.Schedule.Season);
            target.SessionId = source.ResultId; MapToSessionInfoDTO(source.Session);
            target.LocationId = source.Session?.LocationId;
            target.SimSessionDetails = MapToSimSessionDetailsDTO(source.IRSimSessionDetails);

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
            target.CompletedLaps = (int)source.CompletedLaps;
            target.FastestLapTime = TimeSpanConverter.Convert(source.FastestLapTime);
            target.FastLapNr = source.FastLapNr;
            target.FinishPosition = (int)source.FinishPosition;
            target.Incidents = (int)source.Incidents;
            target.Interval = TimeSpanConverter.Convert(source.Interval);
            target.LeadLaps = (int)source.LeadLaps;
            target.MemberId = source.MemberId; // MapToMemberInfoDTO(source.Member);
            target.MemberName = null;
            target.PositionChange = (int)source.PositionChange;
            target.QualifyingTime = TimeSpanConverter.Convert(source.QualifyingTime);
            target.ResultRowId = source.ResultRowId;
            target.ResultId = source.ResultId;
            target.StartPosition = (int)source.StartPosition;
            target.Status = source.Status;
            target.TeamId = source.Team?.TeamId;
            target.TeamName = source.Team?.Name;
            target.LocationId = source.Result.Session.LocationId;
            target.Date = source.Date.GetValueOrDefault();
            target.OldIRating = source.OldIRating;
            target.NewIRating = source.NewIRating;
            target.SeasonStartIRating = source.SeasonStartIRating;
            target.CompletedPct = source.CompletedPct;
            target.OldSafetyRating = source.OldSafetyRating;
            target.NewSafetyRating = source.NewSafetyRating;
            target.OldCpi = source.OldCpi;
            target.NewCpi = source.NewCpi;
            target.OldLicenseLevel = source.OldLicenseLevel;
            target.NewLicenseLevel = source.NewLicenseLevel;

            return target;
        }

        public ScoredResultDataDTO MapToScoredResultDataDTO(ScoredResultEntity source, ScoredResultDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ScoredResultDataDTO();

            MapToResultInfoDTO(source.Result, target);
            target.ScoringId = source.ScoringId; // MapToScoringInfoDTO(source.Scoring);
            target.ScoringName = source.Scoring.Name;
            target.FinalResults = source.FinalResults?.Select(x => MapToScoredResultRowDataDTO(x)).OrderBy(x => x.FinalPosition).ToArray();
            target.CleanesDriverMemberIds = source.CleanestDrivers?.Select(x => x.MemberId).ToArray() ?? new long[0];
            target.HardChargerMemberIds = source.HardChargers?.Select(x => x.MemberId).ToArray() ?? new long[0];
            target.MostPositionsGained = source.FinalResults?.Count == 0 ? -1 : (int)(source.FinalResults?.Max(x => x.FinalPositionChange) ?? -1);
            target.MostPositionsGainedMemberIds = source.FinalResults.Where(x => x.FinalPositionChange != 0 && x.FinalPositionChange == target.MostPositionsGained).Select(x => x.MemberId).ToArray() ?? new long[0];
            target.FastestLapDriverId = source.FastestLapDriver?.MemberId;
            target.FastesLapTime = TimeSpanConverter.Convert(source.FinalResults?.SingleOrDefault(x => x.MemberId == target.FastestLapDriverId)?.FastestLapTime ?? 0);
            target.FastestQualyLapDriver = source.FastestQualyLapDriver?.MemberId;
            target.FastestQualyLapTime = TimeSpanConverter.Convert(source.FinalResults?.SingleOrDefault(x => x.MemberId == target.FastestQualyLapDriver)?.QualifyingTime ?? 0);
            target.FastestAvgLapDriver = source.FastestAvgLapDriver?.MemberId;
            target.FastestAvgLapTime = TimeSpanConverter.Convert(source.FinalResults?.SingleOrDefault(x => x.MemberId == target.FastestAvgLapDriver)?.AvgLapTime ?? 0);

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
            target.BonusPoints = (int)source.BonusPoints;
            target.FinalPosition = source.FinalPosition;
            target.FinalPositionChange = (int)source.FinalPositionChange;
            target.PenaltyPoints = (int)source.PenaltyPoints;
            target.RacePoints = (int)source.RacePoints;
            target.ScoringId = source.ScoringId;
            target.ReviewPenalties = source.ReviewPenalties?.Select(x => MapToReviewPenaltyDTO(x)).ToArray();
            target.TotalPoints = (int)source.TotalPoints;
            target.TeamId = source.TeamId;
            target.TeamName = source.Team?.Name;

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
            target.Description = source.Description;
            target.DropWeeks = source.DropWeeks;
            target.IncPenaltyPoints = source.IncPenaltyPoints;
            target.Name = source.Name;
            target.SeasonId = source.SeasonId; // MapToSeasonInfoDTO(source.Season);
            target.SessionIds = source.Sessions.Select(x => x.SessionId).ToArray();
            target.ResultIds = source.Results.Where(x => x != null).Select(x => x.ResultId).ToArray();
            target.ConnectedScheduleId = source.ConnectedScheduleId; // MapToScheduleInfoDTO(source.ConnectedSchedule);
            target.ScoringKind = source.ScoringKind;
            target.MaxResultsPerGroup = source.MaxResultsPerGroup;
            target.TakeGroupAverage = source.TakeGroupAverage;
            target.ShowResults = source.ShowResults;
            target.ExtScoringSourceId = source.ExtScoringSourceId; // MapToScoringInfoDTO(source.ExtScoringSource);
            target.TakeResultsFromExtSource = source.TakeResultsFromExtSource;
            //target.ResultsFilterOptions = source.ResultsFilterOptions.Select(x => MapToResultsFilterOptionDTO(x)).ToArray();
            target.ResultsFilterOptionIds = source.ResultsFilterOptions.Select(x => x.ResultsFilterId).ToArray();
            target.UseResultSetTeam = source.UseResultSetTeam;
            target.UpdateTeamOnRecalculation = source.UpdateTeamOnRecalculation;
            target.ParentScoringId = source.ParentScoringId;
            target.SubSessionScoringIds = source.SubSessionScorings.Select(x => x.ScoringId).ToArray();
            target.AccumulateBy = source.AccumulateBy;
            target.AccumulateResults = source.AccumulateResultsOption;
            target.ScoringSessionType = source.ScoringSessionType;
            target.SessionSelectType = source.SessionSelectType;
            target.ScoringWeights = source.ScoringWeights;

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
            target.ScoringIds = source.Scorings.Select(x => x.ScoringId).ToArray();
            target.SeasonId = source.SeasonId; // MapToSeasonInfoDTO(source.Season);
            target.SessionIds = source.Sessions.Select(x => x.SessionId).ToArray();
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

            target.CleanestDriverId = source.CleanestDriver?.MemberId; // MapToMemberInfoDTO(source.CleanestDriver);
            target.MostPenaltiesDriverId = source.MostPenaltiesDriver?.MemberId; // MapToMemberInfoDTO(source.MostPenaltiesDriver);
            target.MostPolesDriverId = source.MostPolesDriver?.MemberId; // MapToMemberInfoDTO(source.MostPolesDriver);
            target.MostWinsDriverId = source.MostWinsDriver?.MemberId; // MapToMemberInfoDTO(source.MostWinsDriver);
            target.ScoringId = (source.Scoring?.ScoringId).GetValueOrDefault(); // MapToScoringInfoDTO(source.Scoring);
            target.ScoringTableId = source.ScoringTable.ScoringTableId;
            target.StandingsRows = source.StandingsRows.Select(x => MapToStandingsRowDataDTO(x)).ToArray();
            target.SessionId = source.SessionId;
            target.Name = source.ScoringTable.Name;

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
            target.CompletedLaps = (int)source.CompletedLaps;
            target.CompletedLapsChange = (int)source.CompletedLapsChange;
            target.DroppedResultCount = source.DroppedResultCount;
            target.FastestLaps = source.FastestLaps;
            target.FastestLapsChange = source.FastestLapsChange;
            target.Incidents = (int)source.Incidents;
            target.IncidentsChange = (int)source.IncidentsChange;
            target.LastPosition = source.LastPosition;
            target.LeadLaps = (int)source.LeadLaps;
            target.LeadLapsChange = (int)source.LeadLapsChange;
            target.MemberId = source.Member.MemberId; // MapToMemberInfoDTO(source.Member);
            target.PenaltyPoints = (int)source.PenaltyPoints;
            target.PenaltyPointsChange = (int)source.PenaltyPointsChange;
            target.PolePositions = source.PolePositions;
            target.PolePositionsChange = source.PolePositionsChange;
            target.Position = source.Position;
            target.PositionChange = source.PositionChange;
            target.RacePoints = (int)source.RacePoints;
            target.RacePointsChange = (int)source.RacePointsChange;
            target.Races = source.Races;
            target.RacesCounted = source.RacesCounted;
            //target.ScoringId = source.Scoring.ScoringId; // MapToScoringInfoDTO(source.Scoring);
            target.Top10 = source.Top10;
            target.Top3 = source.Top3;
            target.Top5 = source.Top5;
            target.TotalPoints = (int)source.TotalPoints;
            target.TotalPointsChange = (int)source.TotalPointsChange;
            target.Wins = source.Wins;
            target.WinsChange = source.WinsChange;
            var countedResults = source.CountedResults?.Select(x => MapToScoredResultRowDataDTO(x)).ToArray();
            var droppedResults = source.DroppedResults?.Select(x => MapToScoredResultRowDataDTO(x)).ToArray();
            countedResults.ForEach(x => x.IsDroppedResult = false);
            droppedResults.ForEach(x => x.IsDroppedResult = true);
            target.DriverResults = countedResults.Concat(droppedResults).OrderBy(x => x.Date).ToArray();
            target.TeamId = source.Team?.TeamId;
            target.SeasonStartIRating = target.DriverResults?.FirstOrDefault()?.SeasonStartIRating ?? 0;

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
            target.BonusPoints = (int)source.BonusPoints;
            target.FinalPosition = source.FinalPosition;
            target.FinalPositionChange = source.FinalPositionChange;
            target.PenaltyPoints = (int)source.PenaltyPoints;
            target.RacePoints = (int)source.RacePoints;
            target.ScoringId = source.ScoringId;
            target.TeamId = source.TeamId;
            target.FastestLapTime = TimeSpanConverter.Convert(source.FastestLapTime);
            target.AvgLapTime = TimeSpanConverter.Convert(source.AvgLapTime);
            target.ScoredResultRows = source.ScoredResultRows.Select(x => MapToScoredResultRowDataDTO(x)).ToArray();
            target.TotalPoints = (int)source.TotalPoints;

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

        public SimSessionDetailsDTO MapToSimSessionDetailsDTO(IRSimSessionDetailsEntity source, SimSessionDetailsDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new SimSessionDetailsDTO();
            }

            target.ResultId = source.ResultId;
            target.IRSubsessionId = source.IRSubsessionId;
            target.IRTrackId = source.IRTrackId;
            target.IRSeasonName = source.IRSeasonName;
            target.IRSeasonYear = source.IRSeasonYear;
            target.IRSeasonQuarter = source.IRSeasonQuarter;
            target.IRRaceWeek = source.IRRaceWeek;
            target.IRSessionId = source.IRSessionId;
            target.LicenseCategory = source.LicenseCategory;
            target.SessionName = source.SessionName;
            target.StartTime = source.StartTime;
            target.EndTime = source.EndTime;
            target.CornersPerLap = source.CornersPerLap;
            target.KmDistPerLap = source.KmDistPerLap;
            target.MaxWeeks = source.MaxWeeks;
            target.EventStrengthOfField = source.EventStrengthOfField;
            target.EventAverageLap = source.EventAverageLap;
            target.EventLapsComplete = source.EventLapsComplete;
            target.NumCautions = source.NumCautions;
            target.NumCautionLaps = source.NumCautionLaps;
            target.NumLeadChanges = source.NumLeadChanges;
            target.TimeOfDay = source.TimeOfDay;
            target.DamageModel = source.DamageModel;

            // Track details
            target.IRTrackId = source.IRTrackId;
            target.TrackName = source.TrackName;
            target.ConfigName = source.ConfigName;
            target.TrackCategoryId = source.TrackCategoryId;
            target.Category = source.Category;

            // Weather details
            target.WeatherType = source.WeatherType;
            target.TempUnits = source.TempUnits;
            target.TempValue = source.TempValue;
            target.RelHumidity = source.RelHumidity;
            target.Fog = source.Fog;
            target.WindDir = source.WindDir;
            target.WindUnits = source.WindUnits;
            target.Skies = source.Skies;
            target.WeatherVarInitial = source.WeatherVarInitial;
            target.WeatherVarOngoing = source.WeatherVarOngoing;
            target.SimStartUTCTime = source.SimStartUTCTime;
            target.SimStartUTCOffset = source.SimStartUTCOffset;

            // Track state details 
            target.LeaveMarbles = source.LeaveMarbles;
            target.PracticeRubber = source.PracticeRubber;
            target.QualifyRubber = source.QualifyRubber;
            target.WarmupRubber = source.WarmupRubber;
            target.RaceRubber = source.RaceRubber;
            target.PracticeGripCompound = source.PracticeGripCompound;
            target.QualifyGripCompund = source.QualifyGripCompund;
            target.WarmupGripCompound = source.WarmupGripCompound;
            target.RaceGripCompound = source.RaceGripCompound;

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
            target.Session = GetSessionBaseEntity(new SessionInfoDTO() { SessionId = source.SessionId });
            target.Season = target.Session.Schedule?.Season;
            if (target.Season == null)
            {
                target.Season = target.Session.ParentSession?.Schedule?.Season;
            }
            MapCollection(source.RawResults, target.RawResults, MapToResultRowEntity, x => new object[] { x.ResultRowId, x.ResultId }, autoAddMissing: true, removeFromCollection: true, removeFromDatabase: true);
            MapCollection(source.ReviewIds.Select(x => new IncidentReviewInfoDTO() { ReviewId = x }), target.Reviews, GetReviewEntity, x => x.ReviewId);
            target.RequiresRecalculation = true;
            target.IRSimSessionDetails = MapToSimSessionDetailsEntity(source.SimSessionDetails, target.IRSimSessionDetails);
            if (target.RawResults?.Count > 0)
            {
                var validLapTimes = target.RawResults?.Select(x => x.QualifyingTime).Where(x => x > 0);
                target.PoleLaptime = validLapTimes?.Count() > 0 ? validLapTimes.Min() : 0;
            }


            if (target.RawResults != null)
            {
                foreach (var resultRow in target.RawResults)
                {
                    //compare with other resultrows in this season and determine SeasonStartIRating
                    DbContext.Configuration.LazyLoadingEnabled = false;
                    DbContext.Set<ResultEntity>()
                        .Where(x => x.SeasonId == target.Season.SeasonId)
                        .Include(x => x.RawResults)
                        .Include(x => x.Session)
                        .Load();
                    var seasonResultRows = DbContext.Set<ResultRowEntity>().Local;

                    if (seasonResultRows.Any(x => x.MemberId == resultRow.Member.MemberId && x.Date < target.Session.Date))
                    {
                        resultRow.SeasonStartIRating = seasonResultRows.First(x => x.MemberId == resultRow.Member.MemberId && x.Date < target.Session.Date).SeasonStartIRating;
                    }
                    else
                    {
                        resultRow.SeasonStartIRating = resultRow.OldIRating;
                    }
                    resultRow.PointsEligible = true;
                    DbContext.Configuration.LazyLoadingEnabled = true;
                }
            }

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
            target.Member = GetMemberEntity(new LeagueMemberInfoDTO() { MemberId = source.MemberId });
            target.Team = DefaultGet<TeamEntity>(source.TeamId);
            target.PositionChange = source.PositionChange;
            target.QualifyingTime = TimeSpanConverter.Convert(source.QualifyingTime);
            target.StartPosition = source.StartPosition;
            target.Status = source.Status;
            target.Result = GetResultEntity(new ResultInfoDTO() { ResultId = source.ResultId });
            target.OldIRating = source.OldIRating;
            target.NewIRating = source.NewIRating;
            target.CompletedPct = source.CompletedPct;
            target.OldSafetyRating = source.OldSafetyRating;
            target.NewSafetyRating = source.NewSafetyRating;
            target.OldLicenseLevel = source.OldLicenseLevel;
            target.NewLicenseLevel = source.NewLicenseLevel;
            target.OldCpi = source.OldCpi;
            target.NewCpi = source.NewCpi;

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
            target.Description = source.Description;
            target.DropWeeks = source.DropWeeks;
            target.IncPenaltyPoints = source.IncPenaltyPoints;
            target.Name = source.Name;
            if (target.Season == null && source.SeasonId != 0)
            {
                //target.Season = GetSeasonEntity(new SeasonInfoDTO() { SeasonId = source.SeasonId });
                target.Season = DefaultGet<SeasonEntity>(source.SeasonId);
            }
            if (target.Sessions == null)
                target.Sessions = new List<Entities.Sessions.SessionBaseEntity>();
            else
                MapCollection(source.SessionIds.Select(x => new SessionInfoDTO() { SessionId = x }), target.Sessions, GetSessionBaseEntity, x => x.SessionId, removeFromCollection: true);
            //target.ConnectedSchedule = GetScheduleEntity(source.ConnectedScheduleId != null ? new ScheduleInfoDTO() { ScheduleId = source.ConnectedScheduleId } : null);
            target.ConnectedSchedule = source.SessionSelectType == iRLeagueManager.Enums.ScoringSessionSelectionEnum.SelectSchedule ? DefaultGet<ScheduleEntity>(source.ConnectedScheduleId) : null;
            if (target.ConnectedSchedule != null)
            {
                target.Sessions = target.ConnectedSchedule.Sessions;
            }
            target.ScoringKind = source.ScoringKind;
            target.MaxResultsPerGroup = source.MaxResultsPerGroup;
            target.TakeGroupAverage = source.TakeGroupAverage;
            target.ShowResults = source.ShowResults;
            //target.ExtScoringSource = GetScoringEntity(source.ExtScoringSourceId != null ? new ScoringInfoDTO() { ScoringId = source.ExtScoringSourceId } : null);
            target.ExtScoringSource = DefaultGet<ScoringEntity>(source.ExtScoringSourceId);
            target.TakeResultsFromExtSource = source.TakeResultsFromExtSource;
            target.GetAllSessions().Where(x => x.SessionResult != null).ForEach(x => x.SessionResult.RequiresRecalculation = true);
            target.UseResultSetTeam = source.UseResultSetTeam;
            target.UpdateTeamOnRecalculation = source.UpdateTeamOnRecalculation;
            if (source.SubSessionScoringIds != null)
            {
                MapCollection(source.SubSessionScoringIds.Select(x => new ScoringInfoDTO() { ScoringId = x }), target.SubSessionScorings, x => DefaultGet<ScoringEntity>(x),
                removeFromCollection: true);
                target.ScoringWeights = source.ScoringWeights;
            }
            else
            {
                target.SubSessionScorings = null;
                target.ScoringWeights = null;
            }
            target.SessionSelectType = source.SessionSelectType;
            target.ScoringSessionType = source.ScoringSessionType;
            target.AccumulateBy = source.AccumulateBy;
            target.AccumulateResultsOption = source.AccumulateResults;

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
            MapCollection(source.ScoringIds.Select(x => new ScoringInfoDTO() { ScoringId = x }), target.Scorings, GetScoringEntity, x => x.Keys, removeFromCollection: true);
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
            target.TotalPoints = source.TotalPoints;

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
            if (target.ScoredResultRow?.ScoredResult?.Result != null)
            {
                target.ScoredResultRow.ScoredResult.Result.RequiresRecalculation = true;
            }

            return target;
        }

        public IRSimSessionDetailsEntity MapToSimSessionDetailsEntity(SimSessionDetailsDTO source, IRSimSessionDetailsEntity target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new IRSimSessionDetailsEntity();
            }

            target.IRSubsessionId = source.IRSubsessionId;
            target.IRTrackId = source.IRTrackId;
            target.IRSeasonName = source.IRSeasonName;
            target.IRSeasonYear = source.IRSeasonYear;
            target.IRSeasonQuarter = source.IRSeasonQuarter;
            target.IRRaceWeek = source.IRRaceWeek;
            target.IRSessionId = source.IRSessionId;
            target.LicenseCategory = source.LicenseCategory;
            target.SessionName = source.SessionName;
            target.StartTime = source.StartTime;
            target.EndTime = source.EndTime;
            target.CornersPerLap = source.CornersPerLap;
            target.KmDistPerLap = source.KmDistPerLap;
            target.MaxWeeks = source.MaxWeeks;
            target.EventStrengthOfField = source.EventStrengthOfField;
            target.EventAverageLap = source.EventAverageLap;
            target.EventLapsComplete = source.EventLapsComplete;
            target.NumCautions = source.NumCautions;
            target.NumCautionLaps = source.NumCautionLaps;
            target.NumLeadChanges = source.NumLeadChanges;
            target.TimeOfDay = source.TimeOfDay;
            target.DamageModel = source.DamageModel;

            // Track details
            target.IRTrackId = source.IRTrackId;
            target.TrackName = source.TrackName;
            target.ConfigName = source.ConfigName;
            target.TrackCategoryId = source.TrackCategoryId;
            target.Category = source.Category;

            // Weather details
            target.WeatherType = source.WeatherType;
            target.TempUnits = source.TempUnits;
            target.TempValue = source.TempValue;
            target.RelHumidity = source.RelHumidity;
            target.Fog = source.Fog;
            target.WindDir = source.WindDir;
            target.WindUnits = source.WindUnits;
            target.Skies = source.Skies;
            target.WeatherVarInitial = source.WeatherVarInitial;
            target.WeatherVarOngoing = source.WeatherVarOngoing;
            target.SimStartUTCTime = source.SimStartUTCTime;
            target.SimStartUTCOffset = source.SimStartUTCOffset;

            // Track state details 
            target.LeaveMarbles = source.LeaveMarbles;
            target.PracticeRubber = source.PracticeRubber;
            target.QualifyRubber = source.QualifyRubber;
            target.WarmupRubber = source.WarmupRubber;
            target.RaceRubber = source.RaceRubber;
            target.PracticeGripCompound = source.PracticeGripCompound;
            target.QualifyGripCompund = source.QualifyGripCompund;
            target.WarmupGripCompound = source.WarmupGripCompound;
            target.RaceGripCompound = source.RaceGripCompound;

            return target;
        }
    }
}
