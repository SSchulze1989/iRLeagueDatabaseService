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

using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public class LeagueStatisticSetEntity : StatisticSetEntity
    {
        /// <summary>
        /// List of <see cref="StatisticSetEntity"/> that are used as data source for calculating the league statistic
        /// </summary>
        public virtual List<StatisticSetEntity> StatisticSets { get; set; }

        [ForeignKey(nameof(CurrentChamp))]
        public long? CurrentChampId { get; set; }
        public LeagueMemberEntity CurrentChamp { get; set; }

        public LeagueStatisticSetEntity()
        {
            StatisticSets = new List<StatisticSetEntity>();
        }

        /// <summary>
        /// Calculate statistic data based on the current data set.
        /// <para>Make sure either lazy-loading is enabled on the context or run <see cref="LoadRequiredDataAsync(LeagueDbContext, bool)"/> before execution.</para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework</param>
        public override void Calculate(LeagueDbContext dbContext)
        {
            // Get all statistic rows and group them by member
            var seasonStatisticRows = StatisticSets.SelectMany(x => x.DriverStatistic).GroupBy(x => x.Member);

            List<DriverStatisticRowEntity> removeDriverStatisticRows = DriverStatistic.ToList();

            foreach(var memberStatisticRows in seasonStatisticRows)
            {
                var member = memberStatisticRows.Key;
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

                // Calculate accumulative statistics                 
                foreach (var statisticRow in memberStatisticRows)
                {
                    driverStatRow.CompletedLaps += statisticRow.CompletedLaps;
                    driverStatRow.DrivenKm += statisticRow.DrivenKm;
                    driverStatRow.FastestLaps += statisticRow.FastestLaps;
                    driverStatRow.Incidents += statisticRow.Incidents;
                    driverStatRow.LeadingKm += statisticRow.LeadingKm;
                    driverStatRow.LeadingLaps += statisticRow.LeadingLaps;
                    driverStatRow.PenaltyPoints += statisticRow.PenaltyPoints;
                    driverStatRow.Poles += statisticRow.Poles;
                    driverStatRow.Races += statisticRow.Races;
                    driverStatRow.RacesCompleted += statisticRow.RacesCompleted;
                    driverStatRow.RacesInPoints += statisticRow.RacesInPoints;
                    driverStatRow.Top10 += statisticRow.Top10;
                    driverStatRow.Top15 += statisticRow.Top15;
                    driverStatRow.Top20 += statisticRow.Top20;
                    driverStatRow.Top25 += statisticRow.Top25;
                    driverStatRow.Top3 += statisticRow.Top3;
                    driverStatRow.Top5 += statisticRow.Top5;
                    driverStatRow.Wins += statisticRow.Wins;
                    driverStatRow.RacePoints += statisticRow.RacePoints;
                    driverStatRow.TotalPoints += statisticRow.TotalPoints;
                    driverStatRow.BonusPoints += statisticRow.BonusPoints;
                    driverStatRow.HardChargerAwards += statisticRow.HardChargerAwards;
                    driverStatRow.CleanestDriverAwards += statisticRow.CleanestDriverAwards;
                    driverStatRow.Titles += statisticRow.Titles;
                }

                // Calculate min/max statistics
                driverStatRow.BestFinalPosition = memberStatisticRows.Min(x => x.BestFinalPosition);
                driverStatRow.BestFinishPosition = memberStatisticRows.Min(x => x.BestFinishPosition);
                driverStatRow.BestStartPosition = memberStatisticRows.Min(x => x.BestStartPosition);
                driverStatRow.WorstFinalPosition = memberStatisticRows.Max(x => x.WorstFinalPosition);
                driverStatRow.WorstFinishPosition = memberStatisticRows.Max(x => x.WorstFinishPosition);
                driverStatRow.WorstStartPosition = memberStatisticRows.Max(x => x.WorstStartPosition);

                // Calculate start/end statistics
                var firstResultRow = memberStatisticRows.OrderBy(x => x.FirstRaceDate).FirstOrDefault();
                var lastResultRow = memberStatisticRows.OrderBy(x => x.LastRaceDate).LastOrDefault();
                driverStatRow.FirstResult = firstResultRow.FirstResult;
                driverStatRow.LastResult = lastResultRow.LastResult;
                driverStatRow.FirstRace = firstResultRow.FirstRace;
                driverStatRow.FirstRaceDate = memberStatisticRows.Min(x => x.FirstRaceDate);
                driverStatRow.FirstRaceFinalPosition = firstResultRow.FirstRaceFinalPosition;
                driverStatRow.FirstRaceFinishPosition = firstResultRow.FirstRaceFinishPosition;
                driverStatRow.FirstRaceStartPosition = firstResultRow.FirstRaceStartPosition;
                driverStatRow.FirstSession = memberStatisticRows.Select(x => x.FirstSession).Where(x => x != null).OrderBy(x => x.Date).FirstOrDefault();
                driverStatRow.FirstSessionDate = memberStatisticRows.Min(x => x.FirstSessionDate);
                driverStatRow.LastRace = lastResultRow.LastRace;
                driverStatRow.LastRaceDate = memberStatisticRows.Max(x => x.LastRaceDate);
                driverStatRow.LastRaceFinalPosition = lastResultRow.LastRaceFinalPosition;
                driverStatRow.LastRaceFinishPosition = lastResultRow.LastRaceFinishPosition;
                driverStatRow.LastRaceStartPosition = lastResultRow.LastRaceStartPosition;
                driverStatRow.LastSession = memberStatisticRows.Select(x => x.LastSession).Where(x => x != null).LastOrDefault();
                driverStatRow.LastSessionDate = memberStatisticRows.Max(x => x.LastSessionDate);
                driverStatRow.StartIRating = firstResultRow.StartIRating;
                driverStatRow.EndIRating = lastResultRow.EndIRating;
                driverStatRow.StartSRating = firstResultRow.StartSRating;
                driverStatRow.EndSRating = lastResultRow.EndSRating;
                driverStatRow.CurrentSeasonPosition = lastResultRow.CurrentSeasonPosition;

                // Calculate average statistics
                driverStatRow.AvgFinalPosition = memberStatisticRows.WeightedAverage(x => x.AvgFinalPosition, x => x.Races);
                driverStatRow.AvgFinishPosition = memberStatisticRows.WeightedAverage(x => x.AvgFinishPosition, x => x.Races);
                driverStatRow.AvgIncidentsPerKm = (driverStatRow.Incidents / driverStatRow.DrivenKm).GetZeroWhenInvalid();
                driverStatRow.AvgIncidentsPerLap = ((double)driverStatRow.Incidents / driverStatRow.CompletedLaps).GetZeroWhenInvalid();
                driverStatRow.AvgIncidentsPerRace = ((double)driverStatRow.Incidents / driverStatRow.Races).GetZeroWhenInvalid();
                driverStatRow.AvgIRating = memberStatisticRows.WeightedAverage(x => x.AvgIRating, x => x.Races);
                driverStatRow.AvgPenaltyPointsPerKm = (driverStatRow.PenaltyPoints / driverStatRow.DrivenKm).GetZeroWhenInvalid();
                driverStatRow.AvgPenaltyPointsPerLap = ((double)driverStatRow.PenaltyPoints / driverStatRow.CompletedLaps).GetZeroWhenInvalid();
                driverStatRow.AvgPenaltyPointsPerRace = ((double)driverStatRow.PenaltyPoints / driverStatRow.Races).GetZeroWhenInvalid();
                driverStatRow.AvgPointsPerRace = ((double)driverStatRow.TotalPoints / driverStatRow.Races).GetZeroWhenInvalid();
                driverStatRow.AvgSRating = memberStatisticRows.WeightedAverage(x => x.AvgSRating, x => x.Races);
                driverStatRow.AvgStartPosition = memberStatisticRows.WeightedAverage(x => x.AvgStartPosition, x => x.Races);
            }

            // Calculate current champ
            var seasonStatisticSets = StatisticSets.OfType<SeasonStatisticSetEntity>().Where(x => x.IsSeasonFinished);
            var importedStatisticSets = StatisticSets.OfType<ImportedStatisticSetEntity>();
            var lastSeasonStatistic = seasonStatisticSets.OrderBy(x => x.Season.SeasonEnd).LastOrDefault();
            var lastImportedStatistic = importedStatisticSets.OrderBy(x => x.LastDate).LastOrDefault();
            if (lastSeasonStatistic?.Season.SeasonEnd >= lastImportedStatistic?.LastDate)
            {
                CurrentChamp = lastSeasonStatistic?.DriverStatistic?.SingleOrDefault(x => x.CurrentSeasonPosition == 1)?.Member;
            }
            else
            {
                CurrentChamp = lastImportedStatistic?.DriverStatistic?.SingleOrDefault(x => x.CurrentSeasonPosition == 1)?.Member;
            }

            DriverStatistic.Remove(removeDriverStatisticRows);
        }

        /// <summary>
        /// <para>Load all data required for calculating the statistics from the database.</para>
        /// <para>Must be called prior to <see cref="Calculate"/> if lazy loading is disabled!</para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework.</param>
        /// <param name="force">Force loading data again even if IsDataLoaded is true.</param>
        /// <returns></returns>
        public override async Task LoadRequiredDataAsync(LeagueDbContext dbContext, bool force = false)
        {
            if (IsDataLoaded && force == false)
            {
                return;
            }

            await base.LoadRequiredDataAsync(dbContext, force);

            // Load season statistic sets
            await dbContext.Entry(this).Collection(x => x.StatisticSets).LoadAsync();

            // Load data for each SeasonStatisticSet
            foreach (var statisticSet in StatisticSets)
            {
                await statisticSet.LoadRequiredDataAsync(dbContext);
            }

            IsDataLoaded = true;
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

            RequiresRecalculation |= StatisticSets.Any(x => x.LastModifiedOn > LastModifiedOn);
            if (RequiresRecalculation)
            {
                return true;
            }

            RequiresRecalculation |= (await Task.WhenAll(StatisticSets.Select(async x => await x.CheckRequireRecalculationAsync(dbContext)))).Any(x => x);

            return RequiresRecalculation;
        }
    }
}
