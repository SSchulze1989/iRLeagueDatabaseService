using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Extensions;
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
    public class SeasonStatisticSetEntity : Revision
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Season))]
        public long SeasonId { get; set; }
        public virtual SeasonEntity Season { get; set; }

        public virtual List<ScoringEntity> Scorings { get; set; }

        [ForeignKey(nameof(DriverStatistic))]
        public long DriverStatisticId { get; set; }
        public virtual DriverStatisticEntity DriverStatistic { get; set; }

        /// <summary>
        /// Load the required data to perform results calculation from the database.
        /// Must be performed before calling Calculate() function if lazy loading is disabled.
        /// </summary>
        /// <param name="dbContext">Database context to load data from</param>
        public async Task LoadRequiredDataAsync(LeagueDbContext dbContext)
        {
            await dbContext.Entry(this)
                .Collection(x => x.Scorings)
                .LoadAsync();

            await dbContext.Set<ScoredResultEntity>()
                .Where(x => Scorings.Select(y => y.ScoringId).Contains(x.ScoringId))
                .Include(x => x.Result.Session)
                .Include(x => x.Result.RawResults.Select(y => y.Member))
                .Include(x => x.Result.IRSimSessionDetails)
                .Include(x => x.FinalResults)
                .LoadAsync();

            dbContext.ChangeTracker.DetectChanges();
        }

        /// <summary>
        /// Calculate season statistics. Make sure that either lazy loading is enabled or that all required data is loaded from the database.
        /// </summary>
        /// <param name="dbContext"></param>
        public void Calculate(LeagueDbContext dbContext)
        {
            // Get all scored races
            var scoredRaces = Scorings.Where(x => x.Season.SeasonId == Season.SeasonId).SelectMany(x => x.GetAllSessions()).Where(x => x.SessionResult != null);

            // Get races that need recalculation and recalculate results
            scoredRaces.Select(x => x.SessionResult).Where(x => x.RequiresRecalculation).ForEach(x => x.Session.Scorings.ForEach(y => y.CalculateResults(x.Session.SessionId, dbContext)));

            // Get all scored resultrows and group by driver
            var scoredResultsRows = Scorings.SelectMany(x => x.ScoredResults).SelectMany(x => x.FinalResults).OrderBy(x => x.ResultRow.Date).GroupBy(x => x.ResultRow.Member);

            if (DriverStatistic == null)
            {
                DriverStatistic = new DriverStatisticEntity();
            }

            List<DriverStatisticRowEntity> newDriverStatisticRows = new List<DriverStatisticRowEntity>();
            List<DriverStatisticRowEntity> removeDriverStatisticRows = DriverStatistic.StatisticRows.ToList();

            // Calculate statistics per driver
            foreach(var memberResultRows in scoredResultsRows)
            {
                var member = memberResultRows.Key;
                DriverStatisticRowEntity driverStatRow;
                if (DriverStatistic.StatisticRows.Any(x => x.Member == member))
                {
                    driverStatRow = DriverStatistic.StatisticRows.SingleOrDefault(x => x.Member == member);
                }
                else
                {
                    driverStatRow = new DriverStatisticRowEntity()
                    {
                        Member = member
                    };
                }

                if (memberResultRows.Count() == 0)
                {
                    continue;
                }

                // Calculate accumulative statistics                 
                foreach (var resultRow in memberResultRows)
                {
                    driverStatRow.CompletedLaps += resultRow.ResultRow.CompletedLaps;
                    driverStatRow.DrivenKm += resultRow.ResultRow.CompletedLaps * resultRow.ResultRow.Result.IRSimSessionDetails.KmDistPerLap;
                    driverStatRow.FastestLaps += resultRow.ScoredResult.FastestAvgLapDriver == member ? 1 : 0;
                    driverStatRow.Incidents += resultRow.ResultRow.Incidents;
                    driverStatRow.LeadingKm += resultRow.ResultRow.LeadLaps * resultRow.ResultRow.Result.IRSimSessionDetails.KmDistPerLap;
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
                }

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
                driverStatRow.FirstResult = firstResult.ScoredResult;
                driverStatRow.LastResult = lastResult.ScoredResult;
                driverStatRow.FirstRace = memberResultRows.Select(x => x.ScoredResult.Result.Session).OfType<RaceSessionEntity>().FirstOrDefault();
                driverStatRow.FirstRaceFinalPosition = firstResult.FinalPosition;
                driverStatRow.FirstRaceFinishPosition = firstResult.ResultRow.FinishPosition;
                driverStatRow.FirstRaceId = driverStatRow.FirstRace?.RaceId ?? 0;
                driverStatRow.FirstRaceStartPosition = firstResult.ResultRow.StartPosition;
                driverStatRow.FirstSession = memberResultRows.Select(x => x.ScoredResult.Result.Session).FirstOrDefault();
                driverStatRow.FirstSessionId = driverStatRow.FirstSession?.SessionId ?? 0;
                driverStatRow.LastRace = memberResultRows.Select(x => x.ScoredResult.Result.Session).OfType<RaceSessionEntity>().LastOrDefault();
                driverStatRow.LastRaceId = driverStatRow.LastRace?.RaceId ?? 0;
                driverStatRow.LastRaceFinalPosition = lastResult.FinalPosition;
                driverStatRow.LastRaceFinishPosition = lastResult.ResultRow.FinishPosition;
                driverStatRow.LastRaceStartPosition = lastResult.ResultRow.StartPosition;
                driverStatRow.LastSession = memberResultRows.Select(x => x.ScoredResult.Result.Session).LastOrDefault();
                driverStatRow.LastSessionId = driverStatRow.LastSession?.SessionId ?? 0;
                driverStatRow.StartIRating = firstResult.ResultRow.OldIRating;
                driverStatRow.EndIRating = lastResult.ResultRow.NewIRating;
                driverStatRow.StartSRating = firstResult.ResultRow.OldSafetyRating;
                driverStatRow.EndSRating = lastResult.ResultRow.NewSafetyRating;

                // Calculate average statistics
                var rowCount = memberResultRows.Count();
                driverStatRow.AvgFinalPosition = memberResultRows.Average(x => x.FinalPosition);
                driverStatRow.AvgFinishPosition = memberResultRows.Average(x => x.ResultRow.FinishPosition);
                driverStatRow.AvgIncidentsPerKm = driverStatRow.Incidents / driverStatRow.DrivenKm;
                driverStatRow.AvgIncidentsPerLap = driverStatRow.Incidents / driverStatRow.CompletedLaps;
                driverStatRow.AvgIncidentsPerRace = driverStatRow.Incidents / driverStatRow.Races;
                driverStatRow.AvgIRating = memberResultRows.Average(x => x.ResultRow.NewIRating);
                driverStatRow.AvgPenaltyPointsPerKm = driverStatRow.PenaltyPoints / driverStatRow.DrivenKm;
                driverStatRow.AvgPenaltyPointsPerLap = driverStatRow.PenaltyPoints / driverStatRow.CompletedLaps;
                driverStatRow.AvgPenaltyPointsPerRace = driverStatRow.PenaltyPoints / driverStatRow.Races;
                driverStatRow.AvgPointsPerRace = driverStatRow.TotalPoints / driverStatRow.Races;
                driverStatRow.AvgSRating = memberResultRows.Average(x => x.ResultRow.NewSafetyRating);
                driverStatRow.AvgStartPosition = memberResultRows.Average(x => x.ResultRow.StartPosition);
            }
        }
    }
}
