using iRLeagueDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public class LeagueStatisticSetEntity : StatisticSetEntity
    {
        public virtual List<SeasonStatisticSetEntity> SeasonStatisticSets { get; set; }

        public override void Calculate(LeagueDbContext dbContext)
        {
            // Get all statistic rows and group them by member
            var seasonStatisticRows = SeasonStatisticSets.SelectMany(x => x.DriverStatistic).GroupBy(x => x.Member);

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
                }

                // Calculate min/max statistics
                driverStatRow.BestFinalPosition = memberStatisticRows.Min(x => x.BestFinalPosition);
                driverStatRow.BestFinishPosition = memberStatisticRows.Min(x => x.BestFinishPosition);
                driverStatRow.BestStartPosition = memberStatisticRows.Min(x => x.BestStartPosition);
                driverStatRow.WorstFinalPosition = memberStatisticRows.Max(x => x.WorstFinalPosition);
                driverStatRow.WorstFinishPosition = memberStatisticRows.Max(x => x.WorstFinishPosition);
                driverStatRow.WorstStartPosition = memberStatisticRows.Max(x => x.WorstStartPosition);

                // Calculate start/end statistics
                var firstResult = memberStatisticRows.Select(x => x.FirstResult).OrderBy(x => x.ResultRow.Result.Session.Date).FirstOrDefault();
                var lastResult = memberStatisticRows.Select(x => x.LastResult).OrderBy(x => x.ResultRow.Result.Session.Date).LastOrDefault();
                driverStatRow.FirstResult = firstResult;
                driverStatRow.LastResult = lastResult;
                driverStatRow.FirstRace = memberStatisticRows.Select(x => x.FirstRace).OrderBy(x => x.Date).FirstOrDefault();
                driverStatRow.FirstRaceFinalPosition = firstResult.FinalPosition;
                driverStatRow.FirstRaceFinishPosition = lastResult.ResultRow.FinishPosition;
                driverStatRow.FirstRaceStartPosition = firstResult.ResultRow.StartPosition;
                driverStatRow.FirstSession = memberStatisticRows.Select(x => x.FirstSession).OrderBy(x => x.Date).FirstOrDefault();
                driverStatRow.LastRace = memberStatisticRows.Select(x => x.LastRace).OrderBy(x => x.Date).LastOrDefault();
                driverStatRow.LastRaceFinalPosition = lastResult.FinalPosition;
                driverStatRow.LastRaceFinishPosition = lastResult.ResultRow.FinishPosition;
                driverStatRow.LastRaceStartPosition = lastResult.ResultRow.StartPosition;
                driverStatRow.LastSession = memberStatisticRows.Select(x => x.LastSession).LastOrDefault();
                driverStatRow.StartIRating = firstResult.ResultRow.OldIRating;
                driverStatRow.EndIRating = lastResult.ResultRow.NewIRating;
                driverStatRow.StartSRating = firstResult.ResultRow.OldSafetyRating;
                driverStatRow.EndSRating = lastResult.ResultRow.NewSafetyRating;

                // Calculate average statistics
                var rowCount = memberStatisticRows.Count();
                driverStatRow.AvgFinalPosition = memberStatisticRows.WeightedAverage(x => x.AvgFinalPosition, x => x.Races);
                driverStatRow.AvgFinishPosition = memberStatisticRows.WeightedAverage(x => x.AvgFinishPosition, x => x.Races);
                driverStatRow.AvgIncidentsPerKm = driverStatRow.Incidents / driverStatRow.DrivenKm;
                driverStatRow.AvgIncidentsPerLap = driverStatRow.Incidents / driverStatRow.CompletedLaps;
                driverStatRow.AvgIncidentsPerRace = driverStatRow.Incidents / driverStatRow.Races;
                driverStatRow.AvgIRating = memberStatisticRows.WeightedAverage(x => x.AvgIRating, x => x.Races);
                driverStatRow.AvgPenaltyPointsPerKm = driverStatRow.PenaltyPoints / driverStatRow.DrivenKm;
                driverStatRow.AvgPenaltyPointsPerLap = driverStatRow.PenaltyPoints / driverStatRow.CompletedLaps;
                driverStatRow.AvgPenaltyPointsPerRace = driverStatRow.PenaltyPoints / driverStatRow.Races;
                driverStatRow.AvgPointsPerRace = driverStatRow.TotalPoints / driverStatRow.Races;
                driverStatRow.AvgSRating = memberStatisticRows.WeightedAverage(x => x.AvgSRating, x => x.Races);
                driverStatRow.AvgStartPosition = memberStatisticRows.WeightedAverage(x => x.AvgStartPosition, x => x.Races);
            }
        }

        public override async Task LoadRequiredDataAsync(LeagueDbContext dbContext)
        {
            // Load season statistic sets
            await dbContext.Entry(this).Reference(x => x.SeasonStatisticSets).LoadAsync();

            // Load data for each SeasonStatisticSet
            foreach(var statisticSet in SeasonStatisticSets)
            {
                await statisticSet.LoadRequiredDataAsync(dbContext);
            }
        }
    }
}
