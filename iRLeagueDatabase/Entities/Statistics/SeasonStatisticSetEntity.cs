// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Extensions;
using iRLeagueManager.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public class SeasonStatisticSetEntity : StatisticSetEntity
    {
        /// <summary>
        /// Id of the connected Season. <para>This is set automatically by the <see cref="LeagueDbContext"/></para>
        /// </summary>
        [ForeignKey(nameof(Season))]
        public long SeasonId { get; set; }

        /// <summary>
        /// Season connected with this statistic.
        /// </summary>
        public virtual SeasonEntity Season { get; set; }

        ///// <summary>
        ///// List of <see cref="ScoringEntity"/> that are used as data source for calculating the season statistic.
        ///// </summary>
        //public virtual List<ScoringEntity> Scorings { get; set; }

        [ForeignKey(nameof(ScoringTable))]
        public long? ScoringTableId { get; set; }
        /// <summary>
        /// <see cref="ScoringTableEntity"/> that are used as data source for calculating the season statistics.
        /// </summary>
        public virtual ScoringTableEntity ScoringTable { get; set; }


        /// <summary>
        /// Number of races with results
        /// </summary>
        public int FinishedRaces { get; set; }

        public bool IsSeasonFinished { get; set; }

        public SeasonStatisticSetEntity()
        {
        }


        public override async Task LoadRequiredDataAsync(LeagueDbContext dbContext, bool force = false)
        {
            if (IsDataLoaded && force == false)
            {
                return;
            }

            await base.LoadRequiredDataAsync(dbContext, force);

            if (Season == null)
            {
                await dbContext.Entry(this)
                .Reference(x => x.Season)
                .LoadAsync();
            }
            //if (DriverStatistic == null || DriverStatistic.Count == 0)
            //{
            //    await dbContext.Entry(this)
            //        .Collection(x => x.DriverStatistic)
            //        .LoadAsync();
            //}

            if (ScoringTableId == null || ScoringTableId == 0)
            {
                return;
            }

            var scoringTable = dbContext.Set<ScoringTableEntity>()
                .Where(x => x.ScoringTableId == ScoringTableId)
                .Include(x => x.Scorings.Select(y => y.ExtScoringSource))
                //.Include(x => x.Scorings.Select(y => y.Sessions.Select(z => z.SessionResult)))
                //.Include(x => x.Scorings.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                //.Include(x => x.Scorings.Select(y => y.ExtScoringSource.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                .FirstOrDefault();
            var loadScoringEntityIds = dbContext.Set<ScoringEntity>().Local.Select(y => y.ScoringId);

            if (loadScoringEntityIds.Count() > 0)
            {
                var loadScorings = dbContext.Set<ScoringEntity>()
                    .Where(x => loadScoringEntityIds.Contains(x.ScoringId))
                    .Include(x => x.Sessions.Select(y => y.SessionResult))
                    .ToList();
                var loadScoredResults = dbContext.Set<ScoredResultEntity>()
                    .Where(x => loadScoringEntityIds.Contains(x.ScoringId))
                    .Include(x => x.FinalResults.Select(y => y.ResultRow.Member))
                    .Include(x => x.CleanestDrivers)
                    .Include(x => x.HardChargers)
                    .ToList();
            }

            var scorings = scoringTable.Scorings;

            foreach(var scoring in scorings)
            {
                await dbContext.Entry(scoring)
                    .Reference(x => x.ConnectedSchedule)
                    .Query()
                    .Include(x => x.Sessions)
                    .LoadAsync();
                await dbContext.Entry(scoring)
                    .Collection(x => x.Sessions)
                    .LoadAsync();
                scoring.GetAllSessions();
            }

            var scoringIds = scorings.Select(y => y.ScoringId);

            await dbContext.Set<ScoredResultEntity>()
                .Where(x => scoringIds.Contains(x.ScoringId))
                .Include(x => x.FinalResults.Select(y => y.AddPenalty))
                .Include(x => x.FinalResults.Select(y => y.ReviewPenalties))
                .LoadAsync();

            var resultIds = scorings.SelectMany(x => x.ScoredResults.Select(y => y.ResultId));

            await dbContext.Set<ResultEntity>()
                .Where(x => resultIds.Contains(x.ResultId))
                .Include(x => x.RawResults.Select(y => y.Member.Team))
                .Include(x => x.IRSimSessionDetails)
                .Include(x => x.Session.Reviews.Select(y => y.AcceptedReviewVotes.Select(z => z.CustomVoteCat)))
                .LoadAsync();

            dbContext.ChangeTracker.DetectChanges();

            IsDataLoaded = true;
        }

        public override void Calculate(LeagueDbContext dbContext)
        {
            if (ScoringTable == null)
            {
                return;
            }

            // Get standings
            var standings = ScoringTable.GetSeasonStandings(dbContext);

            // Get all connected scorings
            var scorings = ScoringTable.Scorings;

            // Get all scored races
            var scoredRaces = scorings.SelectMany(x => x.GetAllSessions().Where(y => y.SessionResult != null).Select(y => new { x, y })).GroupBy(x => x.x, x => x.y);

            // Get races that need recalculation and recalculate results
            scoredRaces.ForEach(x => x.Where(y => y.SessionResult.RequiresRecalculation).ForEach(y => x.Key.CalculateResults(y.SessionId, dbContext)));

            // Get all scored resultrows and group by driver
            var scoredResultsRows = scorings.SelectMany(x => x.ScoredResults).SelectMany(x => x.FinalResults).OrderBy(x => x.ResultRow.Date).GroupBy(x => x.ResultRow.Member);

            if (DriverStatistic == null)
            {
                DriverStatistic = new List<DriverStatisticRowEntity>();
            }

            List<DriverStatisticRowEntity> removeDriverStatisticRows = DriverStatistic.ToList();

            // Calculate common season statistics
            FinishedRaces = scoredRaces.Count();
            IsSeasonFinished = Season.Finished;

            // Calculate statistics per driver
            foreach(var memberResultRows in scoredResultsRows)
            {
                var member = memberResultRows.Key;
                var memberStandingsRow = standings.StandingsRows.SingleOrDefault(x => x.Member.MemberId == member.MemberId);
                DriverStatisticRowEntity driverStatRow;
                if (DriverStatistic.Any(x => x.Member == member))
                {
                    driverStatRow = DriverStatistic.SingleOrDefault(x => x.Member == member);
                    driverStatRow.ResetStatistic();
                    removeDriverStatisticRows.Remove(driverStatRow);
                }
                else
                {
                    driverStatRow = new DriverStatisticRowEntity()
                    {
                        Member = member
                    };
                    DriverStatistic.Add(driverStatRow);
                }

                if (memberResultRows.Count() == 0)
                {
                    continue;
                }

                driverStatRow.CurrentSeasonPosition = (standings.StandingsRows.SingleOrDefault(x => x.Member.MemberId == member.MemberId)?.Position).GetValueOrDefault(99999);

                // Calculate accumulative statistics                 
                foreach (var resultRow in memberResultRows)
                {
                    var result = resultRow.ResultRow.Result;
                    driverStatRow.CompletedLaps += resultRow.ResultRow.CompletedLaps;
                    driverStatRow.DrivenKm += resultRow.ResultRow.CompletedLaps * resultRow.ResultRow.Result.IRSimSessionDetails?.KmDistPerLap ?? 0;
                    driverStatRow.FastestLaps += resultRow.ScoredResult.FastestLapDriver == member ? 1 : 0;
                    driverStatRow.Incidents += resultRow.ResultRow.Incidents;
                    driverStatRow.LeadingKm += resultRow.ResultRow.LeadLaps * resultRow.ResultRow.Result.IRSimSessionDetails?.KmDistPerLap ?? 0;
                    driverStatRow.LeadingLaps += resultRow.ResultRow.LeadLaps;
                    driverStatRow.PenaltyPoints += resultRow.PenaltyPoints;
                    driverStatRow.Poles += resultRow.ResultRow.StartPosition == 1 ? 1 : 0;
                    driverStatRow.Races += 1;
                    driverStatRow.RacesCompleted += resultRow.ResultRow.Status == iRLeagueManager.Enums.RaceStatusEnum.Running ? 1 : 0;
                    driverStatRow.RacesInPoints += resultRow.RacePoints > 0 ? 1 : 0;
                    driverStatRow.Top10 += resultRow.FinalPosition <= 10 ? 1 : 0;
                    driverStatRow.Top15 += resultRow.FinalPosition <= 15 ? 1 : 0;
                    driverStatRow.Top20 += resultRow.FinalPosition <= 20 ? 1 : 0;
                    driverStatRow.Top25 += resultRow.FinalPosition <= 25 ? 1 : 0;
                    driverStatRow.Top3 += resultRow.FinalPosition <= 3 ? 1 : 0;
                    driverStatRow.Top5 += resultRow.FinalPosition <= 5 ? 1 : 0;
                    driverStatRow.Wins += resultRow.FinalPosition == 1 ? 1 : 0;
                    driverStatRow.RacePoints += resultRow.RacePoints;
                    driverStatRow.TotalPoints += resultRow.TotalPoints;
                    driverStatRow.BonusPoints += resultRow.BonusPoints;
                    driverStatRow.HardChargerAwards += resultRow.ScoredResult.HardChargers.Contains(member) ? 1 : 0;
                    driverStatRow.CleanestDriverAwards += resultRow.ScoredResult.CleanestDrivers.Contains(member) ? 1 : 0;
                    driverStatRow.IncidentsWithPenalty += (resultRow.ReviewPenalties?.Count).GetValueOrDefault();
                }

                // Quick fix for penalty points
                driverStatRow.PenaltyPoints = driverStatRow.RacePoints + driverStatRow.BonusPoints - driverStatRow.TotalPoints;

                // Calculate min/max statistics
                driverStatRow.BestFinalPosition = memberResultRows.Min(x => x.FinalPosition);
                driverStatRow.BestFinishPosition = memberResultRows.Min(x => x.ResultRow.FinishPosition);
                driverStatRow.BestStartPosition = memberResultRows.Min(x => x.ResultRow.StartPosition);
                driverStatRow.WorstFinalPosition = memberResultRows.Max(x => x.FinalPosition);
                driverStatRow.WorstFinishPosition = memberResultRows.Max(x => x.ResultRow.FinishPosition);
                driverStatRow.WorstStartPosition = memberResultRows.Max(x => x.ResultRow.StartPosition);

                // Calculate start/end statistics
                var firstResult = memberResultRows.First();
                var lastResult = memberResultRows.Last();
                driverStatRow.FirstResult = firstResult;
                driverStatRow.LastResult = lastResult;
                driverStatRow.FirstRace = memberResultRows.Select(x => x.ScoredResult.Result.Session).OfType<RaceSessionEntity>().FirstOrDefault();
                driverStatRow.FirstRaceDate = driverStatRow.FirstRace?.Date;
                driverStatRow.FirstRaceFinalPosition = firstResult.FinalPosition;
                driverStatRow.FirstRaceFinishPosition = firstResult.ResultRow.FinishPosition;
                driverStatRow.FirstRaceStartPosition = firstResult.ResultRow.StartPosition;
                driverStatRow.FirstSession = memberResultRows.Select(x => x.ScoredResult.Result.Session).FirstOrDefault();
                driverStatRow.FirstSessionDate = driverStatRow.FirstSession?.Date;
                driverStatRow.LastRace = memberResultRows.Select(x => x.ScoredResult.Result.Session).OfType<RaceSessionEntity>().LastOrDefault();
                driverStatRow.LastRaceDate = driverStatRow.LastRace?.Date;
                driverStatRow.LastRaceFinalPosition = lastResult.FinalPosition;
                driverStatRow.LastRaceFinishPosition = lastResult.ResultRow.FinishPosition;
                driverStatRow.LastRaceStartPosition = lastResult.ResultRow.StartPosition;
                driverStatRow.LastSession = memberResultRows.Select(x => x.ScoredResult.Result.Session).LastOrDefault();
                driverStatRow.LastSessionDate = driverStatRow.LastSession?.Date;
                driverStatRow.StartIRating = firstResult.ResultRow.OldIRating;
                driverStatRow.EndIRating = lastResult.ResultRow.NewIRating;
                driverStatRow.StartSRating = firstResult.ResultRow.OldSafetyRating;
                driverStatRow.EndSRating = lastResult.ResultRow.NewSafetyRating;
                driverStatRow.CurrentSeasonPosition = memberStandingsRow?.Position ?? 0;
                driverStatRow.Titles = IsSeasonFinished && driverStatRow.CurrentSeasonPosition == 1 ? 1 : 0;

                // Calculate average statistics
                driverStatRow.AvgFinalPosition = memberResultRows.Average(x => x.FinalPosition);
                driverStatRow.AvgFinishPosition = memberResultRows.Average(x => x.ResultRow.FinishPosition);
                driverStatRow.AvgIncidentsPerKm = ((double)driverStatRow.Incidents / driverStatRow.DrivenKm).GetZeroWhenInvalid();
                driverStatRow.AvgIncidentsPerLap = ((double)driverStatRow.Incidents / driverStatRow.CompletedLaps).GetZeroWhenInvalid();
                driverStatRow.AvgIncidentsPerRace = ((double)driverStatRow.Incidents / driverStatRow.Races).GetZeroWhenInvalid();
                driverStatRow.AvgIRating = memberResultRows.Average(x => x.ResultRow.NewIRating);
                driverStatRow.AvgPenaltyPointsPerKm = ((double)driverStatRow.PenaltyPoints / driverStatRow.DrivenKm).GetZeroWhenInvalid();
                driverStatRow.AvgPenaltyPointsPerLap = ((double)driverStatRow.PenaltyPoints / driverStatRow.CompletedLaps).GetZeroWhenInvalid();
                driverStatRow.AvgPenaltyPointsPerRace = ((double)driverStatRow.PenaltyPoints / driverStatRow.Races).GetZeroWhenInvalid();
                driverStatRow.AvgPointsPerRace = ((double)driverStatRow.TotalPoints / driverStatRow.Races).GetZeroWhenInvalid();
                driverStatRow.AvgSRating = memberResultRows.Average(x => x.ResultRow.NewSafetyRating);
                driverStatRow.AvgStartPosition = memberResultRows.Average(x => x.ResultRow.StartPosition);
            }

            // Remove rows of drivers that are not in the current statistic set anymore
            removeDriverStatisticRows.ForEach(x => DriverStatistic.Remove(x));
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            base.Delete(dbContext);
        }

        /// <summary>
        /// Perform a check if recalculation is required based on the current data set.
        /// <para>Calling this function will also set the value of <see cref="StatisticSetEntity.RequiresRecalculation"/></para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework</param>
        /// <returns><see langword="true"/> if calculation is required</returns>
        public override async Task<bool> CheckRequireRecalculationAsync(LeagueDbContext dbContext)
        {
            if (RequiresRecalculation)
            {
                return true;
            }

            if (dbContext.Configuration.LazyLoadingEnabled == false)
            {
                await LoadRequiredDataAsync(dbContext);
            }

            var Scorings = ScoringTable?.Scorings;

            RequiresRecalculation = (Scorings
                ?.SelectMany(x => x.Sessions.Where(y => y.SessionResult != null).Select(y => y.SessionResult))
                ?.Any(x => x.Season.Finished == false && (x.RequiresRecalculation || x.LastModifiedOn > LastModifiedOn))).GetValueOrDefault();

            return RequiresRecalculation;
        }
    }
}
