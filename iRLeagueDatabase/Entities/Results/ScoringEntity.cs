﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Reviews;

using iRLeagueDatabase;
using iRLeagueDatabase.Entities.Filters;
using iRLeagueDatabase.Filters;
using System.Runtime.CompilerServices;
using iRLeagueManager.Timing;
using iRLeagueDatabase.Extensions;
using iRLeagueDatabase.Enums;
using System.CodeDom;
using System.Globalization;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Calculation;
using System.Data.SqlClient;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoringEntity : Revision
    {
        [Key]
        public long ScoringId { get; set; }
        [ForeignKey(nameof(ParentScoring))]
        public long? ParentScoringId { get; set; }
        public virtual ScoringEntity ParentScoring { get; set; }
        public ScoringKindEnum ScoringKind { get; set; }
        public SessionType ScoringSessionType { get; set; }
        public ScoringSessionSelectionEnum SessionSelectType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public override object MappingId => ScoringId;
        public int DropWeeks { get; set; }
        public int AverageRaceNr { get; set; }
        public int MaxResultsPerGroup { get; set; }
        public bool TakeGroupAverage { get; set; }
        public bool ShowResults { get; set; }

        [InverseProperty(nameof(ScoringEntity.ExtScoringSource))]
        public virtual List<ScoringEntity> DependendScorings { get; set; }
        [ForeignKey(nameof(ExtScoringSource))]
        public long? ExtScoringSourceId { get; set; }
        public virtual ScoringEntity ExtScoringSource { get; set; }
        public bool TakeResultsFromExtSource { get; set; }
        //public bool IsMultiScoring { get; set; }

        public virtual List<ScoringTableEntity> ScoringTables { get; set; }

        [InverseProperty(nameof(ResultsFilterOptionEntity.Scoring))]
        public virtual List<ResultsFilterOptionEntity> ResultsFilterOptions { get; set; }

        #region SubSessionOptions
        [InverseProperty(nameof(ScoringEntity.ParentScoring))] 
        public virtual List<ScoringEntity> SubSessionScorings { get; set; }
        public string ScoringWeightValues { get; set; }
        [NotMapped]
        public IEnumerable<double> ScoringWeights
        {
            get => ScoringWeightValues?
                .Split(OptionsDelimiter)
                .Select(x => double.TryParse(x, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double val) ? val : 0.0);
            set => ScoringWeightValues = string.Join(OptionsDelimiter.ToString(), value?.Select(x => x.ToString(CultureInfo.InvariantCulture)) ?? new string[0]);
        }

        public AccumulateByOption AccumulateBy { get; set; }

        public AccumulateResultsOption AccumulateResultsOption { get; set; }

        #endregion

        public List<SessionBaseEntity> sessions;
        public virtual List<SessionBaseEntity> Sessions
        {
            get
            {
                return sessions;
            }
            set => sessions = value;
        }
        [NotMapped]
        public virtual IEnumerable<ResultEntity> Results => Sessions?.Select(x => x.SessionResult);

        //[ForeignKey(nameof(Season))]
        //public long SeasonId { get; set; }
        [ForeignKey(nameof(Season))]
        public long SeasonId { get; set; }
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string BasePoints { get; set; }
        public string BonusPoints { get; set; }
        public string IncPenaltyPoints { get; set; }
        //public string MultiScoringFactors { get; set; }
        //public virtual List<ScoringEntity> MultiScoringResults { get; set; }
        //public virtual List<ScoredResultRowEntity> ScoredResultRows { get; set; }
        [InverseProperty(nameof(ScoredResultEntity.Scoring))]
        public virtual List<ScoredResultEntity> ScoredResults { get; set; }
        [ForeignKey(nameof(ConnectedSchedule))]
        public long? ConnectedScheduleId { get; set; }
        public virtual ScheduleEntity ConnectedSchedule { get; set; }
        public bool UseResultSetTeam { get; set; }
        public bool UpdateTeamOnRecalculation { get; set; }

        private const char OptionsDelimiter = ',';
        public string PointsSortOptions { get; set; }
        public string FinalSortOptions { get; set; }

        //public ScoringRuleBase Rule { get; set; }
        public ScoringEntity() 
        { 
        }

        /// <summary>
        /// Get all sessions, including sessions from Multiscoring
        /// </summary>
        /// <returns></returns>
        public List<SessionBaseEntity> GetAllSessions()
        {
            //if (IsMultiScoring && MultiScoringResults != null && MultiScoringResults.Count > 0)
            //{
            //    return MultiScoringResults.Where(x => x.Sessions != null).SelectMany(x => x.Sessions).ToList();
            //}
            UpdateSessionList();
            return Sessions;
        }

        public IEnumerable<SortOption> GetPointsSortOptions()
        {
            var sortOptions = new List<SortOption>();
            if (string.IsNullOrEmpty(PointsSortOptions))
            {
                sortOptions = new List<SortOption>()
                {
                    //new SortOption(SortOptionEnum.Interval, 1),
                    new SortOption(SortOptionEnum.FinishPosition, 1),
                };
            }
            else
            {
                // get values from string
                var optionValues = PointsSortOptions?
                    .Split(OptionsDelimiter)
                    .Select(x => int.TryParse(x, out int val) ? val : 0) ?? new int[0];

                // cast to sort options
                foreach (var value in optionValues)
                {
                    var option = new SortOption((SortOptionEnum)(Math.Abs(value)), Math.Sign(value));
                    sortOptions.Add(option);
                }
            }
            return sortOptions;
        }

        public void SetPointsSortOptions(IEnumerable<SortOption> sortOptions)
        {
            var optionStrings = sortOptions.Select(x => ((int)x.Option * x.SortDirection).ToString());
            PointsSortOptions = string.Join(OptionsDelimiter.ToString(), optionStrings.ToArray());
        }

        public IEnumerable<SortOption> GetFinalSortOptions()
        {
            var sortOptions = new List<SortOption>();
            if (string.IsNullOrEmpty(FinalSortOptions))
            {
                sortOptions = new List<SortOption>()
                {
                    new SortOption(SortOptionEnum.TotalPoints, -1),
                    new SortOption(SortOptionEnum.PenaltyPoints, 1),
                    new SortOption(SortOptionEnum.FinishPosition, 1)
                };
            }
            else
            {
                // get values from string
                var optionValues = FinalSortOptions
                    .Split(OptionsDelimiter)
                    .Select(x => int.TryParse(x, out int val) ? val : 0);

                // cast to sort options
                foreach (var value in optionValues)
                {
                    var option = new SortOption((SortOptionEnum)(Math.Abs(value)), Math.Sign(value));
                    sortOptions.Add(option);
                }
            }
            return sortOptions;
        }
        public void SetFinalSortOptions(IEnumerable<SortOption> sortOptions)
        {
            var optionStrings = sortOptions.Select(x => ((int)x.Option * x.SortDirection).ToString());
            FinalSortOptions = string.Join(OptionsDelimiter.ToString(), optionStrings.ToArray());
        }

        public ScoredResultEntity CalculateResults(long sessionId, LeagueDbContext dbContext)
        {
            if (Season.Finished)
            {
                var scoredResult = ScoredResults.SingleOrDefault(x => x.ResultId == sessionId);
                if (scoredResult != null)
                {
                    scoredResult.Result.RequiresRecalculation = false;
                    return scoredResult;
                }
            }

            UpdateSessionList();
            if (!Sessions.Any(x => x.SessionId == sessionId))
                return null;

            var session = Sessions.SingleOrDefault(x => x.SessionId == sessionId);

            return CalculateResults(session, dbContext);
        }

        private void UpdateSessionList()
        {
            if (ConnectedSchedule != null)
            {
                if (Sessions == null)
                {
                    Sessions = new List<SessionBaseEntity>();
                }
                var remove = Sessions.ToList();

                foreach(var session in ConnectedSchedule.Sessions)
                {
                    if (Sessions.Contains(session) == false)
                    {
                        Sessions.Add(session);
                    }
                    remove.Remove(session);
                }
                remove.ForEach(x => Sessions.Remove(x));
            }
        }

        private IEnumerable<(long MemberId, int IRating)> GetSeasonStartIrating(long seasonId, LeagueDbContext dbContext)
        {
            List<(long MemberId, int IRating)> result = new List<(long MemberId, int IRating)>();
            using (var cmd = dbContext.Database.Connection.CreateCommand())
            {
                cmd.CommandText = 
                    @"with summary 
                    as
                    (
	                    select r.MemberId, r.OldIRating, rss.Season_SeasonId, 
		                    ROW_NUMBER() over(partition by r.MemberId
			                    order by rs.Date) as rank
		                    from dbo.ResultRowEntities r
			                    join dbo.SessionBaseEntities as rs on r.ResultId = rs.SessionId
			                    join dbo.ScheduleEntities as rss on rs.ScheduleId = rss.ScheduleId
		                    where Season_SeasonId = @seasonId
                    )

                    select MemberId, OldIRating
	                    from summary
                    where rank = 1;";

                if (cmd.Connection.State != ConnectionState.Open) 
                { 
                    cmd.Connection.Open(); 
                }
               
                cmd.Parameters.Add(new SqlParameter("seasonId", seasonId));

                using (var dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var memberId = dataReader.GetInt64(0);
                        var iRating = dataReader.GetInt32(1);
                        result.Add((memberId, iRating));
                    }
                }
            }

            return result;
        }

        //public IEnumerable<ScoredResultRowEntity> CalculateResults(SessionBaseEntity session, LeagueDbContext dbContext)
        public ScoredResultEntity CalculateResults(SessionBaseEntity session, LeagueDbContext dbContext)
        {
            if (session == null)
                return null;

            // Check if session is actually in scoring sessions or a subsession
            if (Sessions.Contains(session) == false)
            {
                if (session.SubSessions.Any(x => Sessions.Contains(x)))
                {
                    session = session.SubSessions.First(x => Sessions.Contains(x));
                }
                else
                {
                    return null;
                }
            }

            if (session.SubSessions?.Count > 0)
            {
                if (session.SubSessions.Any(x => x.SessionResult == null))
                {
                    return null;
                }
            }
            else if (session.SessionResult == null)
            {
                return null;
            }

            UpdateSessionList();

            //List<ScoredResultRowEntity> scoredResultRows = new List<ScoredResultRowEntity>();
            var scoredResult = GetCurrentScoredResult(session, dbContext);
            var firstResult = Season.Schedules
                .SelectMany(x => x.Sessions)
                .FirstOrDefault(x => x.SessionResult != null)
                ?.SessionResult;

            var startIRatings = GetSeasonStartIrating(Season.SeasonId, dbContext);
            
            if (scoredResult == null || scoredResult.Result?.RequiresRecalculation == false)
            {
                return scoredResult;
            }

            var reviewVotes = GetReviewVotes(session);

            // Get filters
            if (ResultsFilterOptions == null)
            {
                dbContext.Entry(this).Collection(x => x.ResultsFilterOptions).Load();
            }

            List<IResultsFilter> resultsFilters = new List<IResultsFilter>();
            foreach(var filterOption in ResultsFilterOptions)
            {
                var filter = FilterFactoryHelper.GetFilter(filterOption.ResultsFilterType, filterOption.ColumnPropertyName, filterOption.Exclude, filterOption.FilterPointsOnly, filterOption.Comparator);
                filter.SetFilterValueStrings(filterOption.FilterValues.Split(';'));
                resultsFilters.Add(filter);
            }

            List<ScoredResultRowEntity> scoredResultRows = scoredResult.FinalResults;
            if (TakeResultsFromExtSource && ExtScoringSource != null)
            {
                var extScoredResult = ExtScoringSource.ScoredResults.SingleOrDefault(x => x.ResultId == session.SessionId);
                if (extScoredResult != null)
                {
                    scoredResultRows = extScoredResult.FinalResults;

                    foreach (var scoredResultRow in scoredResult.FinalResults.ToList())
                    {
                        scoredResultRow.Delete(dbContext);
                    }
                    scoredResult.FinalResults.Clear();
                }
            }
            else
            {
                ResultEntity result = session.SessionResult;

                if (scoredResultRows.Count > 0)
                {
                    scoredResultRows.ForEach(x => x.PointsEligible = x.ResultRow.PointsEligible);
                }
                
                // Get Result and result rows -- this will create or update a new result in case of accumulated scoring:
                if (AccumulateResultsOption != AccumulateResultsOption.None && 
                    SubSessionScorings?.Count > 0)
                {
                    scoredResult = CalculateAccumulatedResult(session, dbContext, scoredResult);
                    scoredResultRows = scoredResult.FinalResults;
                    result = scoredResult.Result;
                }

                IEnumerable<ResultRowEntity> resultRows = result.RawResults;

                // Recaclulate completed pct
                var maxLaps = resultRows.Count() > 0 ? resultRows.Max(x => x.CompletedLaps) : 0;
                if (maxLaps > 0)
                {
                    resultRows.ForEach(x => x.CompletedPct = (double)x.CompletedLaps / maxLaps);
                }

                // Apply filters for whole result
                resultRows = FilterRows(resultRows, resultsFilters.Where(x => x.FilterPointsOnly == false));

                IDictionary<int, int> basePoints = new Dictionary<int, int>();
                if (BasePoints != "" && BasePoints != null)
                    basePoints = BasePoints.Split(' ').Select((x, i) => new { Item = int.Parse(x), Index = i }).ToDictionary(x => x.Index + 1, x => x.Item);
                IDictionary<string, int> bonusPoints = new Dictionary<string, int>();
                if (BonusPoints != "" && BonusPoints != null)
                    bonusPoints = BonusPoints.Split(' ').Select(x => new { Item = int.Parse(x.Split(':').Last()), Index = x.Split(':').First() }).ToDictionary(x => x.Index, x => x.Item);

                var removeRows = scoredResultRows.ToList();

                foreach (var resultRowObj in resultRows.Select((row, index) => new { index, row }))
                {
                    var resultRow = resultRowObj.row;
                    var position = resultRowObj.index + 1;
                    ScoredResultRowEntity scoredResultRow;
                    if (scoredResultRows.Exists(x => x.ResultRowId == resultRow.ResultRowId))
                    {
                        scoredResultRow = scoredResultRows.Single(x => x.ResultRowId == resultRow.ResultRowId);
                        removeRows.Remove(scoredResultRow);
                    }
                    else
                    {
                        scoredResultRow = new ScoredResultRowEntity()
                        {
                            ResultRow = resultRow,
                            ReviewPenalties = new List<ReviewPenaltyEntity>(),
                            PointsEligible = resultRow?.PointsEligible ?? true
                            //Scoring = this,
                        };

                        // get team information
                        if (UseResultSetTeam)
                        {
                            scoredResultRow.Team = resultRow.Team;
                        }
                        else
                        {
                            scoredResultRow.Team = resultRow.Member.Team;
                        }
                        scoredResultRows.Add(scoredResultRow);
                    }

                    // update team information on recalculation
                    if (UpdateTeamOnRecalculation)
                    {
                        if (UseResultSetTeam)
                        {
                            scoredResultRow.Team = resultRow.Team;
                        }
                        else
                        {
                            scoredResultRow.Team = resultRow.Member.Team;
                        }
                    }

                    var startIRating = startIRatings.FirstOrDefault(x => x.MemberId == resultRow.MemberId).IRating;
                    resultRow.SeasonStartIRating = startIRating != 0 ? startIRating : resultRow.OldIRating;

                    var scoredResultRowReviewVotes = reviewVotes.Where(x => x.MemberAtFaultId == scoredResultRow.ResultRow.MemberId);
                    if (scoredResultRow.ReviewPenalties != null)
                    {
                        scoredResultRow.ReviewPenalties.Clear();
                        var removePenalty = scoredResultRow.ReviewPenalties.ToList();
                        foreach (var reviewVote in scoredResultRowReviewVotes)
                        {
                            var reviewPenalty = scoredResultRow.ReviewPenalties.SingleOrDefault(x => x.Review?.ReviewId == reviewVote.ReviewId);
                            if (reviewPenalty == null)
                            {
                                reviewPenalty = new ReviewPenaltyEntity()
                                {
                                    Review = reviewVote.IncidentReview,
                                    ScoredResultRow = scoredResultRow,
                                    ReviewVote = reviewVote
                                };
                                scoredResultRow.ReviewPenalties.Add(reviewPenalty);
                            }
                            else
                            {
                                reviewPenalty.ReviewVote = reviewVote;
                                removePenalty.Remove(reviewPenalty);
                            }
                            reviewPenalty.PenaltyPoints += GetReviewPenaltyPoints(reviewVote);
                        }
                        removePenalty.ForEach(x => x.Delete(dbContext));
                        //dbContext.SaveChanges();
                    }
                    scoredResultRow.RacePoints = resultRow.RacePoints;
                    scoredResultRow.PenaltyPoints = GetPenaltyPoints(scoredResultRow);
                    scoredResultRow.PenaltyTime = GetPenaltyTime(scoredResultRow).Ticks;
                    if (BasePoints.Count() > 0)
                    {
                        scoredResultRow.RacePoints = 0;
                        scoredResultRow.BonusPoints = 0;
                    }
                    else if (BonusPoints.Count() > 0)
                    {
                        scoredResultRow.BonusPoints = 0;
                    }
                    scoredResultRow.TotalPoints = scoredResultRow.RacePoints + scoredResultRow.BonusPoints - scoredResultRow.PenaltyPoints;
                }

                // Sort rows before points
                var sortOptions = GetPointsSortOptions();
                scoredResultRows = SortRows(scoredResultRows, sortOptions).ToList();
                // Apply filters for points only
                var pointScoringRows = scoredResultRows.Where(x => x.PointsEligible);
                pointScoringRows = FilterRows(pointScoringRows.ToArray(), resultsFilters.Where(x => x.FilterPointsOnly));

                // Set points eligible = false for excluded rows
                scoredResultRows.Except(pointScoringRows).ForEach(x => x.PointsEligible = false);

                foreach (var scoredResultRowObj in pointScoringRows.Select((row, index) => new { index, row }))
                {
                    var scoredResultRow = scoredResultRowObj.row;
                    var position = scoredResultRowObj.index + 1;
                    var startPosition = scoredResultRowObj.row.StartPosition;
                    var fastestLapPosition = scoredResultRows
                        .Where(x => x.FastestLapTime > 0)
                        .OrderBy(x => x.FastestLapTime)
                        .Select((x, i) => new { index = i, row = x })
                        .SingleOrDefault(x => x.row.MemberId == scoredResultRow.MemberId)
                        ?.index + 1 ?? 0;
                    scoredResultRow.RacePoints = basePoints.ContainsKey(position) ? basePoints[position] : scoredResultRow.RacePoints;
                    scoredResultRow.BonusPoints += bonusPoints.ContainsKey($"p{position}") ? bonusPoints[$"p{position}"] : 0;
                    scoredResultRow.BonusPoints += bonusPoints.ContainsKey($"q{startPosition}") ? bonusPoints[$"q{startPosition}"] : 0;
                    scoredResultRow.BonusPoints += bonusPoints.ContainsKey($"f{fastestLapPosition}") ? bonusPoints[$"f{fastestLapPosition}"] : 0;
                    scoredResultRow.TotalPoints = scoredResultRow.RacePoints + scoredResultRow.BonusPoints - scoredResultRow.PenaltyPoints;
                }

                removeRows.ForEach(x => { x.Delete(dbContext); scoredResultRows.Remove(x); });

                // sort rows after points
                sortOptions = GetFinalSortOptions();
                scoredResultRows = SortRows(scoredResultRows, sortOptions).ToList();

                //var droppedRows = scoredResultRows.Where(x => x.ResultRow.CompletedLaps == 0).ToList();
                //scoredResultRows = scoredResultRows.Except(droppedRows).ToList();
                //dbContext.Set<ScoredResultRowEntity>().RemoveRange(droppedRows.Where(x => dbContext.Entry(x).State != System.Data.Entity.EntityState.Detached));

                //scoredResultRows.Select((x, i) => new { Item = x, Index = i }).ToList().ForEach(x =>
                //{
                //    x.Item.FinalPosition = x.Index + 1;
                //    x.Item.FinalPositionChange = x.Item.ResultRow.StartPosition - x.Item.FinalPosition;
                //});
                if (scoredResultRows.Count > 0)
                {

                    ScoredResultRowEntity previousRow = null;
                    for (int i = 0; i < scoredResultRows.Count(); i++)
                    {
                        var row = scoredResultRows.ElementAt(i);
                        if (previousRow != null && row.TotalPoints != 0 && row.TotalPoints == previousRow.TotalPoints && row.PenaltyPoints == previousRow.PenaltyPoints)
                        {
                            row.FinalPosition = previousRow.FinalPosition;
                        }
                        else
                        {
                            row.FinalPosition = i + 1;
                        }
                        row.FinalPositionChange = row.ResultRow.StartPosition - row.FinalPosition;
                        previousRow = row;
                    }

                    var fastestLapRow = scoredResultRows.Where(x => x.ResultRow.FastestLapTime > 0).MinBy(x => x.ResultRow.FastestLapTime);
                    scoredResult.FastestLap = fastestLapRow?.ResultRow.FastestLapTime ?? 0;
                    scoredResult.FastestLapDriver = fastestLapRow?.ResultRow.Member;

                    var fastestAvgLapRow = scoredResultRows.Where(x => x.ResultRow.AvgLapTime > 0).MinBy(x => x.ResultRow.AvgLapTime);
                    scoredResult.FastestAvgLap = fastestAvgLapRow?.ResultRow.AvgLapTime ?? 0;
                    scoredResult.FastestAvgLapDriver = fastestAvgLapRow?.ResultRow.Member;

                    var fastestQualyLapRow = scoredResultRows.Where(x => x.ResultRow.QualifyingTime > 0).MinBy(x => x.ResultRow.QualifyingTime);
                    scoredResult.FastestQualyLap = fastestQualyLapRow?.ResultRow.QualifyingTime ?? 0;
                    scoredResult.FastestQualyLapDriver = fastestQualyLapRow?.ResultRow.Member;
                }

                dbContext.SaveChanges();
            }

            if (ScoringKind == ScoringKindEnum.Team)
            {
                // Group scored result rows by team

                var groupedResultRows = scoredResultRows.GroupBy(x => x.Team);
                var scoredTeamResult = (ScoredTeamResultEntity)scoredResult;
                
                if (dbContext.Entry(scoredTeamResult).State != System.Data.Entity.EntityState.Added)
                    dbContext.Entry(scoredTeamResult).Collection(x => x.TeamResults).Query()
                    .Include(x => x.ScoredResultRows).Load();

                if (scoredTeamResult.TeamResults == null)
                    scoredTeamResult.TeamResults = new List<ScoredTeamResultRowEntity>();

                var removeRows = scoredTeamResult.TeamResults.ToList();

                foreach (var teamGroup in groupedResultRows.Where(x => x.Key != null))
                {
                    var team = teamGroup.Key;
                    var teamResultRow = scoredTeamResult.TeamResults.Where(x => x.Team != null).SingleOrDefault(x => x.Team.TeamId == team.TeamId);
                    if (teamResultRow == null)
                    {
                        teamResultRow = new ScoredTeamResultRowEntity();
                        scoredTeamResult.TeamResults.Add(teamResultRow);
                    }
                    else
                    {
                        removeRows.Remove(teamResultRow);
                    }
                    teamResultRow.RemoveAllRows();
                    teamGroup.AggregateTeamResults(maxRacesCount: MaxResultsPerGroup, teamResultRow);
                    if (teamResultRow.ScoredResultRows.Count > 0)
                    {
                        teamResultRow.Date = scoredResultRows.First().ResultRow.Date;
                        teamResultRow.ClassId = scoredResultRows.First().ResultRow.ClassId;
                        teamResultRow.CarClass = scoredResultRows.First().ResultRow.CarClass;
                        teamResultRow.AvgLapTime = teamResultRow.ScoredResultRows.Select(x => x.ResultRow.AvgLapTime).Sum() / teamResultRow.ScoredResultRows.Count();
                        teamResultRow.FastestLapTime = teamResultRow.ScoredResultRows.Select(x => x.ResultRow.FastestLapTime).Min();
                    }
                }

                removeRows.ForEach(x => x.Delete(dbContext));
                scoredTeamResult.TeamResults.Remove(removeRows);

                var scoredTeamResultRows = scoredTeamResult.TeamResults.OrderByDescending(x => x.TotalPoints);
                for (int i = 0; i < scoredTeamResultRows.Count(); i++)
                {
                    scoredTeamResultRows.ElementAt(i).FinalPosition = i + 1;
                }
            }

            // Calculate hard chargers and cleanest drivers
            var contenders = GetHardChargerContenders(scoredResult);
            if (contenders.Count() > 0)
            {
                var maxPlacesGained = contenders.Max(x => x.FinalPositionChange);
                var hardChargers = contenders.Where(x => x.FinalPositionChange == maxPlacesGained).Select(x => x.ResultRow.Member);
                scoredResult.HardChargers.RemoveAll(x => hardChargers.Contains(x) == false);
                scoredResult.HardChargers.AddRange(hardChargers.Except(scoredResult.HardChargers));
            }
            contenders = GetCleanesDriverContenders(scoredResult);
            if (contenders.Count() > 0)
            {
                var minIncidents = contenders.Min(x => x.ResultRow.Incidents);
                var cleanestDrivers = contenders.Where(x => x.ResultRow.Incidents == minIncidents).Select(x => x.ResultRow.Member);
                scoredResult.CleanestDrivers.RemoveAll(x => cleanestDrivers.Contains(x) == false);
                scoredResult.CleanestDrivers.AddRange(cleanestDrivers.Except(scoredResult.CleanestDrivers));
            }
            //return scoredResultRows;

            // set parent session result to require recalculation
            if (session.ParentSession?.SessionResult != null)
            {
                session.ParentSession.SessionResult.RequiresRecalculation = true;
            }

            return scoredResult;
        }

        /// <summary>
        /// Calculate the accumulated results based on the configured SubSessionScorings
        /// </summary>
        /// <param name="session">Session entity to calculate the result for</param>
        /// <param name="dbContext">Current Database Context</param>
        /// <returns>Result Entity for the session; <see langword="null"/> if session was empty</returns>
        private ScoredResultEntity CalculateAccumulatedResult(SessionBaseEntity session, LeagueDbContext dbContext, ScoredResultEntity accScoredResult)
        {
            if (session == null)
            {
                return null;
            }

            var startIRatings = GetSeasonStartIrating(Season.SeasonId, dbContext);

            // Check Result existence and if recalc is required
            ResultEntity accResult = session.SessionResult;
            if (accResult == null)
            {
                accResult = new ResultEntity()
                {
                    Session = session,
                    Season = session.Schedule.Season,
                    CreatedByUserId = "ScoringService",
                    CreatedOn = DateTime.Now,
                    RequiresRecalculation = true
                };
            }
            if (accResult.RequiresRecalculation == false && accScoredResult != null)
            {
                return accScoredResult;
            }
            if (accScoredResult == null)
            {
                accScoredResult = new ScoredResultEntity()
                {
                    Scoring = this,
                    CreatedByUserId = "ScoringService",
                    CreatedOn = DateTime.Now,
                    Result = accResult
                };
            }
            accScoredResult.Result = accResult;

            // Get the results to accumulate
            var subSessions = session.SubSessions;
            var subSessionResults = subSessions
                .SelectMany(x => x.SessionResult.ScoredResults)
                .Where(x => SubSessionScorings.Contains(x.Scoring));
            var subSessionResultRows = subSessionResults.SelectMany(x => x.FinalResults);

            if (subSessionResultRows.Count() == 0)
            {
                accScoredResult.FinalResults = subSessionResultRows.ToList();
                return accScoredResult;
            }

            // Make sure every driver in this event has a result row for every subsession
            // only drivers that have participated in all events will be scored
            var eventDrivers = subSessionResults
                .SelectMany(x => x.Result.DriverList)
                //.Aggregate((x, y) => x.Intersect(y))
                .Distinct();

            var driverResultRows = subSessionResultRows
                .Where(x => eventDrivers.Contains(x.Member))
                .GroupBy(x => x.Member);

            // clear rows
            var removeRows = accResult.RawResults.Where(x => eventDrivers.Contains(x.Member) == false).ToList();
            removeRows.ForEach(x => 
            { 
                x.Delete(dbContext); accResult.RawResults.Remove(x); 
            });
            var removeScoredRows = accScoredResult.FinalResults.Where(x => eventDrivers.Contains(x.Member) == false).ToList();
            removeScoredRows.ForEach(x =>
            {
                x.Delete(dbContext);
                accScoredResult.FinalResults.Remove(x);
            });

            // Calculate
            List<ScoredResultRowEntity> accResultRows = new List<ScoredResultRowEntity>();
            var totalLaps = subSessionResults.Sum(x => x.FinalResults.Max(y => y.CompletedLaps));

            foreach (var rows in driverResultRows)
            {
                var driver = rows.Key;
                var accumulator = new Accumulator()
                {
                    AccumulateOption = AccumulateResultsOption,
                    AccumulateWeights = ScoringWeights
                };

                var accResultRow = accResult.RawResults.SingleOrDefault(x => x.Member == driver);
                if (accResultRow == null)
                {
                    accResultRow = new ResultRowEntity()
                    {
                        Result = accResult,
                        Member = driver
                    };
                    if (UseResultSetTeam)
                    {
                        accResultRow.Team = rows.Last().Team;
                    }
                    else
                    {
                        accResultRow.Team = driver.Team;
                    }
                    accResult.RawResults.Add(accResultRow);
                }
                else if (UpdateTeamOnRecalculation)
                {
                    if (UseResultSetTeam)
                    {
                        accResultRow.Team = rows.Last().Team;
                    }
                    else
                    {
                        accResultRow.Team = driver.Team;
                    }
                }

                //accResultRow.AvgLapTime = accumulator.Accumulate(rows.Select(x => x.AvgLapTime), AccumulateResultsOption.WeightedAverage, GetBestOption.MinValue, rows.Select(x => x.CompletedLaps));
                accResultRow.StartPosition = rows.OrderBy(x => x.ResultRow.Result.Session.Date?.TimeOfDay).FirstOrDefault()?.StartPosition ?? 0;
                accResultRow.AvgLapTime = 0;
                accResultRow.Car = rows.First().Car;
                accResultRow.CarClass = rows.First().CarClass;
                accResultRow.CarId = rows.First().CarId;
                accResultRow.CarNumber = rows.First().CarNumber;
                accResultRow.ClassId = rows.First().ClassId;
                accResultRow.ClubId = rows.First().ClubId;
                accResultRow.ClubName = rows.First().ClubName;
                accResultRow.CompletedLaps = rows.Sum(x => x.CompletedLaps);
                accResultRow.NewCpi = rows.Last().NewCpi;
                accResultRow.OldCpi = rows.First().OldCpi;
                accResultRow.NewIRating = rows.Last().NewIRating;
                accResultRow.OldIRating = rows.First().OldIRating;
                accResultRow.NewLicenseLevel = rows.Last().NewLicenseLevel;
                accResultRow.OldLicenseLevel = rows.First().OldLicenseLevel;
                accResultRow.NewSafetyRating = rows.Last().NewSafetyRating;
                accResultRow.OldSafetyRating = rows.First().OldSafetyRating;
                accResultRow.NumOfftrackLaps = rows.Sum(x => x.NumOfftrackLaps);
                accResultRow.NumContactLaps = rows.Sum(x => x.NumContactLaps);
                accResultRow.NumPitStops = rows.Sum(x => x.NumPitStops);
                accResultRow.FastLapNr = 0;
                accResultRow.FastestLapTime = accumulator.Accumulate(rows.Select(x => x.FastestLapTime), AccumulateResultsOption.Best, GetBestOption.MinValue);
                accResultRow.FinishPosition = accumulator.Accumulate(rows.Select(x => x.FinishPosition), GetBestOption.MinValue);
                accResultRow.CompletedPct = accumulator.Accumulate(rows.Select(x => x.CompletedPct), AccumulateResultsOption.WeightedAverage, GetBestOption.MaxValue, rows.Select(x => x.ResultRow.Result.CompletedLaps / totalLaps));
                accResultRow.Division = rows.Last().Division;
                accResultRow.Incidents = rows.Sum(x => x.Incidents);
                //accResultRow.Interval = (new LapInterval(TimeSpan.Zero, (int)(maxCompletedLaps - accResultRow.CompletedLaps))).Time.Ticks;
                //accResultRow.Interval = accumulator.Accumulate(rows.Select(x => x.Interval), GetBestOption.MinValue);
                accResultRow.LeadLaps = rows.Sum(x => x.LeadLaps);
                accResultRow.License = rows.Last().License;
                //accResultRow.QualifyingTime = accumulator.Accumulate(rows.Select(x => x.QualifyingTime), GetBestOption.MinValue);
                accResultRow.QualifyingTime = rows.FirstOrDefault()?.QualifyingTime ?? 0;
                accResultRow.PointsEligible = rows.Select(x => x.PointsEligible).Aggregate((x, y) => x |= y);

                var startIRating = startIRatings.FirstOrDefault(x => x.MemberId == accResultRow.MemberId).IRating;
                accResultRow.SeasonStartIRating = startIRating != 0 ? startIRating : accResultRow.OldIRating;

                var accScoredResultRow = accScoredResult.FinalResults.SingleOrDefault(x => x.Member == driver);
                if (accScoredResultRow == null)
                {
                    accScoredResultRow = new ScoredResultRowEntity()
                    {
                        ResultRow = accResultRow,
                        ScoredResult = accScoredResult
                    };
                    if (UseResultSetTeam)
                    {
                        accScoredResultRow.Team = accResultRow.Team;
                    }
                    else
                    {
                        accScoredResultRow.Team = driver.Team;
                    }
                    accScoredResult.FinalResults.Add(accScoredResultRow);
                }
                else if (UpdateTeamOnRecalculation)
                {
                    if (UseResultSetTeam)
                    {
                        accScoredResultRow.Team = accResultRow.Team;
                    }
                    else
                    {
                        accScoredResultRow.Team = driver.Team;
                    }
                }
                 
                accScoredResultRow.AddPenalty = null;
                accScoredResultRow.BonusPoints = accumulator.Accumulate(rows.Select(x => x.BonusPoints), GetBestOption.MaxValue, ScoringWeights);
                accScoredResultRow.FinalPositionChange = 0;
                accScoredResultRow.PenaltyPoints = accumulator.Accumulate(rows.Select(x => x.PenaltyPoints), AccumulateResultsOption.Sum);
                accScoredResultRow.RacePoints = accumulator.Accumulate(rows.Select(x => x.RacePoints), GetBestOption.MaxValue, ScoringWeights);
                accScoredResultRow.FinalPosition = 0;
                accScoredResultRow.TotalPoints = 0;
                accScoredResultRow.ResultRow = accResultRow;
                accScoredResultRow.PointsEligible = rows.Select(x => x.PointsEligible).Aggregate((x, y) => x |= y);
            }



            // Calculate lap interval
            var maxCompletedLaps = accScoredResult.FinalResults.Max(x => x.CompletedLaps);
            accScoredResult.FinalResults.ForEach(x => x.ResultRow.Interval = (new LapInterval(TimeSpan.Zero, (int)(maxCompletedLaps - x.CompletedLaps))).Time.Ticks);

            dbContext.SaveChanges();

            return accScoredResult;
        }

        private ScoredResultEntity GetCurrentScoredResult(SessionBaseEntity session, LeagueDbContext dbContext)
        {
            var scoredResult = ScoredResults.SingleOrDefault(x => x.ResultId == session.SessionId);

            if (Season.Finished && scoredResult != null)
            {
                scoredResult.Result.RequiresRecalculation = false;
                return scoredResult;
            }

            if (ScoringKind == ScoringKindEnum.Member)
            {
                if (scoredResult == null)
                {
                    scoredResult = new ScoredResultEntity()
                    {
                        Result = session.SessionResult,
                        Scoring = this,
                        FinalResults = new List<ScoredResultRowEntity>(),
                    };
                    ScoredResults.Add(scoredResult);
                }
            }
            else if (ScoringKind == ScoringKindEnum.Team)
            {
                if (scoredResult != null && scoredResult is ScoredTeamResultEntity == false)
                {
                    ScoredResults.Remove(scoredResult);
                    scoredResult.Delete(dbContext);
                    dbContext.SaveChanges();
                    scoredResult = null;
                }

                if (scoredResult == null)
                {
                    scoredResult = new ScoredTeamResultEntity()
                    {
                        Result = session.SessionResult,
                        Scoring = this,
                        FinalResults = new List<ScoredResultRowEntity>(),
                        TeamResults = new List<ScoredTeamResultRowEntity>()
                    };
                    ScoredResults.Add(scoredResult);
                }
            }
            else
            {
                if (scoredResult != null)
                {
                    ScoredResults.Remove(scoredResult);
                    scoredResult.Delete(dbContext);
                    scoredResult = null;
                }
            }

            return scoredResult;
        }

        private IEnumerable<AcceptedReviewVoteEntity> GetReviewVotes(SessionBaseEntity session)
        {
            var reviewVotes = new List<AcceptedReviewVoteEntity>();

            if (session.Reviews != null)
            {
                foreach (var review in session.Reviews)
                {
                    if (review.AcceptedReviewVotes != null && review.AcceptedReviewVotes.Count > 0)
                    {
                        reviewVotes.AddRange(review.AcceptedReviewVotes);
                    }
                }
            }

            return reviewVotes;
        }

        private IEnumerable<T> FilterRows<T>(IEnumerable<T> resultRows, IEnumerable<IResultsFilter> resultsFilters) where T : IResultRow
        {
            foreach (var filter in resultsFilters)
            {
                resultRows = filter.GetFilteredRows(resultRows);
            }
            return resultRows;
        }

        private IEnumerable<T> SortRows<T>(IEnumerable<T> rows, IEnumerable<SortOption> sortOptions) where T : IResultRow
        {
            foreach(var option in sortOptions.Reverse())
            {
                switch (option.Option)
                {
                    case SortOptionEnum.AverageLap:
                        rows = rows.OrderBy(x => x.AvgLapTime * option.SortDirection);
                        break;
                    case SortOptionEnum.BonusPoints:
                        rows = rows.OrderBy(x => x.BonusPoints * option.SortDirection);
                        break;
                    case SortOptionEnum.FastestLap:
                        rows = rows.OrderBy(x => x.FastestLapTime * option.SortDirection);
                        break;
                    case SortOptionEnum.FinishPosition:
                        rows = rows.OrderBy(x => x.FinishPosition * option.SortDirection);
                        break;
                    case SortOptionEnum.Interval:
                        rows = rows.OrderBy(x => x.Interval * option.SortDirection);
                        break;
                    case SortOptionEnum.PenaltyPoints:
                        rows = rows.OrderBy(x => x.PenaltyPoints * option.SortDirection);
                        break;
                    case SortOptionEnum.RacePoints:
                        rows = rows.OrderBy(x => x.FastestLapTime * option.SortDirection);
                        break;
                    case SortOptionEnum.StartPosition:
                        rows = rows.OrderBy(x => x.StartPosition * option.SortDirection);
                        break;
                    case SortOptionEnum.TotalPoints:
                        rows = rows.OrderBy(x => x.TotalPoints * option.SortDirection);
                        break;
                    default:
                        break;
                }
            }
            return rows;
        }

        private IEnumerable<ScoredResultRowEntity> GetHardChargerContenders(ScoredResultEntity scoredResult)
        {
            return scoredResult.FinalResults.Where(x => x.ResultRow.QualifyingTime > 0 && x.ResultRow.Status == RaceStatusEnum.Running && x.ResultRow.QualifyingTime <= x.ResultRow.Result.PoleLaptime * 1.04);
        }

        private IEnumerable<ScoredResultRowEntity> GetCleanesDriverContenders(ScoredResultEntity scoredResult)
        {
            return scoredResult.FinalResults.Where(x => (new LapInterval(iRLeagueManager.Timing.TimeSpanConverter.Convert(x.ResultRow.Interval))).Laps == 0 && x.ResultRow.Status == RaceStatusEnum.Running);
        }

        public IEnumerable<ScoredResultEntity> GetResultsFromSource()
        {
            if (TakeResultsFromExtSource && ExtScoringSource != null)
                return ExtScoringSource.ScoredResults;
            else
                return ScoredResults;
        }

        private int GetReviewPenaltyPoints(ReviewVoteEntity vote)
        {
            if (vote.CustomVoteCat != null)
            {
                return vote.CustomVoteCat.DefaultPenalty;
            }

            switch (vote.Vote)
            {
                case VoteEnum.Kat0:
                    return 0;
                case VoteEnum.Kat1:
                    return 1;
                case VoteEnum.Kat2:
                    return 3;
                default:
                    return 0;
            }
        }

        private int GetPenaltyPoints(ScoredResultRowEntity scoredResultRow)
        {
            if (scoredResultRow == null)
                return 0;

            int penaltyPoints = 0;

            if (scoredResultRow.AddPenalty != null)
            {
                penaltyPoints += scoredResultRow.AddPenalty.PenaltyPoints;
            }

            if (scoredResultRow.ReviewPenalties != null)
            {
                foreach(var reviewPenalty in scoredResultRow.ReviewPenalties)
                {
                    penaltyPoints += reviewPenalty.PenaltyPoints;
                }
            }

            return penaltyPoints;
        }

        private TimeSpan GetPenaltyTime(ScoredResultRowEntity scoredResultRow)
        {
            if (scoredResultRow == null)
            {
                return TimeSpan.Zero;
            }

            TimeSpan penaltyTime = TimeSpan.Zero;

            if (scoredResultRow.AddPenalty != null)
            {
                penaltyTime += TimeSpan.FromTicks(scoredResultRow.AddPenalty.PenaltyTime);
            }

            if (scoredResultRow.ReviewPenalties != null)
            {
                foreach(var reviewPenalty in scoredResultRow.ReviewPenalties)
                {
                    penaltyTime += TimeSpan.FromTicks(reviewPenalty.PenaltyTime);
                }
            }

            return penaltyTime;
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            ScoredResults?.ToList().ForEach(x => x.Delete(dbContext));
            Sessions?.ForEach(x => x.Scorings.Remove(this));
            DependendScorings?.ForEach(x => x.ExtScoringSource = null);
            ScoringTables?.ForEach(x => x.Scorings.Remove(this));
            SubSessionScorings?.ToList().ForEach(x => x.ParentScoring = null);
            base.Delete(dbContext);
        }
    }

    public static partial class ScoredResultExtensions
    {
        public static IEnumerable<StandingsRowEntity> Diff<T>(this IEnumerable<T> source, IEnumerable<T> compare) where T : StandingsRowEntity
        {
            List<StandingsRowEntity> resultList = new List<StandingsRowEntity>();
            foreach (var row in source)
            {
                StandingsRowEntity standingsRow;
                T compRow;
                if (typeof(T).Equals(typeof(TeamStandingsRowEntity)))
                {
                    compRow = compare.SingleOrDefault(x => (x as TeamStandingsRowEntity).Team.TeamId == (row as TeamStandingsRowEntity).Team.TeamId);
                }
                else
                {
                    compRow = compare.SingleOrDefault(x => x.Member.MemberId == row.Member.MemberId);
                }
                if (compRow != null)
                {
                    if (row is TeamStandingsRowEntity teamStandingsRow && compRow is TeamStandingsRowEntity compTeamStandingsRow)
                    {
                        standingsRow = teamStandingsRow.Diff(compTeamStandingsRow);
                    }
                    else
                    { 
                        standingsRow = row.Diff(compRow); 
                    }
                }
                else
                {
                    standingsRow = row;
                }

                resultList.Add(standingsRow);
            }

            return resultList;
        }

        public static StandingsRowEntity AggregateResults<T>(this IEnumerable<T> source, int maxRacesCount = 0, bool canDropPenaltyRace = true) where T : ScoredResultRowEntity
        {
            source = source.OrderBy(x => x.ResultRow.Date).OrderBy(x => -x.TotalPoints);

            if (!canDropPenaltyRace)
            {
                source = source.OrderBy(x => !(x.PenaltyPoints != 0));
            }

            var standingsRow = new StandingsRowEntity
            {
                //Scoring = source.First().ScoredResult.Scoring,
                Member = source.First().ResultRow.Member,
                ClassId = source.Last().ResultRow.ClassId,
                CarClass = source.Last().ResultRow.CarClass,
                Team = source.Last().Team
            };

            standingsRow.AddRows(source.Skip(maxRacesCount).ToArray(), countPoints: false);
            standingsRow.AddRows(source.Take(maxRacesCount).ToArray(), countPoints: true);
            standingsRow.DroppedResultCount = standingsRow.DroppedResults.Count;

            return standingsRow;
        }

        public static ScoredTeamResultRowEntity AggregateTeamResults<T>(this IGrouping<Members.TeamEntity, T> source, int maxRacesCount = 0, ScoredTeamResultRowEntity teamResultsRow = null) where T : ScoredResultRowEntity
        {
            if (teamResultsRow == null)
            {
                teamResultsRow = new ScoredTeamResultRowEntity();
            }
            if (teamResultsRow.ScoredResultRows == null)
            {
                teamResultsRow.ScoredResultRows = new List<ScoredResultRowEntity>();
            }

            teamResultsRow.Team = source.Key;
            var resultRows = source.OrderByDescending(x => x.TotalPoints).Take(maxRacesCount).ToList();
            //teamResultsRow.RemoveAllRows();
            foreach (var resultRow in teamResultsRow.ScoredResultRows)
            {
                if (resultRows.Exists(x => x.ScoredResultRowId == resultRow.ScoredResultRowId) == false)
                {
                    teamResultsRow.ScoredResultRows.Remove(resultRow);
                }
            }
            foreach (var resultRow in resultRows)
            {
                if (teamResultsRow.ScoredResultRows.Exists(x => x.ScoredResultRowId == resultRow.ScoredResultRowId) == false)
                {
                    teamResultsRow.ScoredResultRows.Add(resultRow);
                }
            }

            teamResultsRow.AddRows(resultRows);

            return teamResultsRow;
        }

        public static IEnumerable<StandingsRowEntity> AggregateByDriver(this IEnumerable<ScoredResultEntity> source, int maxRacesCount = 0, bool canDropPenaltyRace = true)
        {
            return source.SelectMany(x => x.FinalResults).AggregateByDriver(maxRacesCount, canDropPenaltyRace);
        }

        public static IEnumerable<StandingsRowEntity> AggregateByDriver<T>(this IEnumerable<T> source, int maxRacesCount = -1, bool canDropPenaltyRace = true) where T : ScoredResultRowEntity
        {
            var driverStandingsRows = new List<StandingsRowEntity>();

            //foreach (var scoredResultRow in source)
            //{
            //    StandingsRowEntity standingsRow;
            //    if (driverStandingsRows.Exists(x => x.Member.MemberId == scoredResultRow.ResultRow.Member.MemberId))
            //    {
            //        standingsRow = driverStandingsRows.Single(x => x.Member.MemberId == scoredResultRow.ResultRow.Member.MemberId);
            //        scoredResultRow.ResultRow.ClassId = scoredResultRow.ResultRow.ClassId;
            //        scoredResultRow.ResultRow.CarClass = scoredResultRow.ResultRow.CarClass;
            //    }
            //    else
            //    {
            //        standingsRow = new StandingsRowEntity()
            //        {
            //            Member = scoredResultRow.ResultRow.Member,
            //            ClassId = scoredResultRow.ResultRow.ClassId,
            //            CarClass = scoredResultRow.ResultRow.CarClass
            //        };
            //    }

            //    standingsRow.AddRows(scoredResultRow);
            //}

            var drivers = source.Select(x => x.ResultRow.Member).Distinct();

            foreach (var driver in drivers)
            {
                var driverResultRows = source.Where(x => x.ResultRow.Member == driver);
                driverStandingsRows.Add(driverResultRows.AggregateResults(maxRacesCount, canDropPenaltyRace));
            }

            return driverStandingsRows;
        }
    }

    public static class EnumExtenstions
    {
        public static T MaxBy<T, TSelect>(this IEnumerable<T> source, Func<T, TSelect> selector) where TSelect : IComparable
        {
            T maxValue = source.FirstOrDefault();
            foreach(var value in source)
            {
                var temp = selector(value).CompareTo(selector(maxValue)) >= 0;
                if (temp)
                    maxValue = value;
            }
            //return source.Aggregate((x, y) => selector(x).CompareTo(selector(y)) >= 0 ? x : y);
            return maxValue;
        }

        public static T MinBy<T, TSelect>(this IEnumerable<T> source, Func<T, TSelect> selector) where TSelect : IComparable
        {
            if (source.Count() == 0)
            {
                return default;
            }
            return source.Aggregate((x, y) => selector(x).CompareTo(selector(y)) <= 0 ? x : y);
        }
    }


}
