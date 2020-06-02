using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Entities.Sessions;

using iRLeagueDatabase;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoringEntity : Revision
    {
        [Key]
        public long ScoringId { get; set; }
        public string Name { get; set; }
        public override object MappingId => ScoringId;
        public int DropWeeks { get; set; }
        public int AverageRaceNr { get; set; }

        public List<SessionBaseEntity> sessions;
        public virtual List<SessionBaseEntity> Sessions
        {
            get
            {
                if (ConnectedSchedule != null)
                {
                    return ConnectedSchedule.Sessions;
                }
                else
                {
                    return sessions;
                }
            }
            set => sessions = value;
        }
        [NotMapped]
        public virtual IEnumerable<ResultEntity> Results => Sessions.Select(x => x.SessionResult);

        //[ForeignKey(nameof(Season))]
        //public long SeasonId { get; set; }
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string BasePoints { get; set; }
        public string BonusPoints { get; set; }
        public string IncPenaltyPoints { get; set; }
        public string MultiScoringFactors { get; set; }
        public virtual List<ScoringEntity> MultiScoringResults { get; set; }
        //public virtual List<ScoredResultRowEntity> ScoredResultRows { get; set; }
        [InverseProperty(nameof(ScoredResultEntity.Scoring))]
        public virtual List<ScoredResultEntity> ScoredResults { get; set; }
        public virtual ScheduleEntity ConnectedSchedule { get; set; }

        //public ScoringRuleBase Rule { get; set; }
        public ScoringEntity() 
        { 
        }

        public StandingsEntity GetSeasonStandings()
        {
            var session = Sessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();
            if (session == null)
                return null;

            return GetSeasonStandings(session.SessionId);
        }

        public StandingsEntity GetSeasonStandings(long sessionId)
        {
            var currentSession = Sessions.SingleOrDefault(x => x.SessionId == sessionId);

            if (currentSession == null)
                currentSession = Sessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();

            if (currentSession == null)
                return null;

            var previousSessions = Sessions.Where(x => x.Date < currentSession.Date).OrderBy(x => x.Date);
            var previousResults = previousSessions.Where(x => x.SessionResult != null).Select(x => x.SessionResult);
            var previousScoredResults = ScoredResults;

            var currentResult = currentSession.SessionResult;
            var currentScoredResult = ScoredResults.SingleOrDefault(x => x.Result.Session == currentSession);

            StandingsEntity standings = new StandingsEntity();
            List<StandingsRowEntity> previousStandingsRows = new List<StandingsRowEntity>();
            List<StandingsRowEntity> currentStandingsRows = new List<StandingsRowEntity>();

            // Get unique Standings row per driver
            var drivers = previousSessions.SelectMany(x => x.SessionResult.DriverList).Concat(currentSession.SessionResult.DriverList).Distinct();
            foreach (var driver in drivers)
            {
                var standingsRow = new StandingsRowEntity
                {
                    Member = driver
                };
                previousStandingsRows.Add(standingsRow);


                var driverPreviousResults = ScoredResults
                    .Where(x => x.Result.DriverList.Contains(driver));
                var driverPreviousResultRows = driverPreviousResults
                    .Select(x => x.FinalResults.SingleOrDefault(y => y.ResultRow.Member == driver))
                    .OrderBy(x => -x.TotalPoints).ToArray();
                var driverCurrentResultRow = currentScoredResult?.FinalResults.SingleOrDefault(x => x.ResultRow.Member == driver);

                var driverDropRaces = driverPreviousResultRows.Count() - (Sessions.Count() - DropWeeks) + 1;
                bool dropCurrentRace = false;

                var driverScoredResultRows = driverPreviousResultRows;

                if (driverDropRaces > 0)
                {
                    if (driverPreviousResultRows[-driverDropRaces].TotalPoints >= driverCurrentResultRow.TotalPoints)
                    {
                        dropCurrentRace = true;
                        driverDropRaces--;
                    }
                    driverScoredResultRows = driverScoredResultRows.Take(driverScoredResultRows.Count() - driverDropRaces).ToArray();
                }

                foreach (var scoredResult in driverPreviousResults)
                {
                    standingsRow.FastestLaps += (scoredResult.FastestLapDriver == driver) ? 1 : 0;
                }

                foreach (var scoredResultRow in driverScoredResultRows)
                {
                    standingsRow.CompletedLaps += scoredResultRow.ResultRow.CompletedLaps;
                    standingsRow.Incidents += scoredResultRow.ResultRow.Incidents;
                    standingsRow.PenaltyPoints += scoredResultRow.PenaltyPoints;
                    standingsRow.PolePositions += (scoredResultRow.ResultRow.StartPosition == 1) ? 1 : 0;
                    standingsRow.RacePoints += scoredResultRow.RacePoints;
                    standingsRow.Top10 += (scoredResultRow.FinalPosition <= 10) ? 1 : 0;
                    standingsRow.Top5 += (scoredResultRow.FinalPosition <= 5) ? 1 : 0;
                    standingsRow.Top3 += (scoredResultRow.FinalPosition <= 3) ? 1 : 0;
                    standingsRow.TotalPoints += scoredResultRow.TotalPoints;
                    standingsRow.Wins += (scoredResultRow.FinalPosition == 1) ? 1 : 0;
                }

                // Correct Total points for not dropped Penalties:
                standingsRow.PenaltyPoints = driverPreviousResultRows.Sum(x => x.PenaltyPoints);

                StandingsRowEntity currentStandingsRow;
                if (!dropCurrentRace)
                {
                    currentStandingsRow = new StandingsRowEntity
                    {
                        Member = driver,
                        CarClass = driverCurrentResultRow.ResultRow.CarClass,
                        ClassId = driverCurrentResultRow.ResultRow.ClassId
                    };
                    currentStandingsRow.CompletedLapsChange = driverCurrentResultRow.ResultRow.CompletedLaps;
                    currentStandingsRow.CompletedLaps += driverCurrentResultRow.ResultRow.CompletedLaps;
                    currentStandingsRow.IncidentsChange = driverCurrentResultRow.ResultRow.Incidents;
                    currentStandingsRow.Incidents += driverCurrentResultRow.ResultRow.Incidents;
                    currentStandingsRow.PenaltyPointsChange = driverCurrentResultRow.PenaltyPoints;
                    currentStandingsRow.PenaltyPoints += driverCurrentResultRow.PenaltyPoints;
                    currentStandingsRow.PolePositions += (driverCurrentResultRow.ResultRow.StartPosition == 1) ? 1 : 0;
                    currentStandingsRow.RacePointsChange = driverCurrentResultRow.RacePoints;
                    currentStandingsRow.RacePoints += driverCurrentResultRow.RacePoints;
                    currentStandingsRow.Top10 += (driverCurrentResultRow.FinalPosition <= 10) ? 1 : 0;
                    currentStandingsRow.Top5 += (driverCurrentResultRow.FinalPosition <= 5) ? 1 : 0;
                    currentStandingsRow.Top3 += (driverCurrentResultRow.FinalPosition <= 3) ? 1 : 0;
                    currentStandingsRow.TotalPointsChange = currentStandingsRow.TotalPoints + driverCurrentResultRow.TotalPoints;
                    currentStandingsRow.TotalPoints += driverCurrentResultRow.TotalPoints;
                    currentStandingsRow.Wins += (driverCurrentResultRow.FinalPosition == 1) ? 1 : 0;
                }
                else
                {
                    currentStandingsRow = standingsRow;
                }
                currentStandingsRows.Add(currentStandingsRow);
            }

            previousStandingsRows = previousStandingsRows.OrderBy(x => -x.TotalPoints).ToList();
        }

        public IEnumerable<ScoredResultRowEntity> CalculateResults(long sessionId, LeagueDbContext dbContext)
        {
            if (!Sessions.Any(x => x.SessionId == sessionId))
                return null;

            var session = Sessions.SingleOrDefault(x => x.SessionId == sessionId);

            return CalculateResults(session, dbContext);
        }

        public IEnumerable<ScoredResultRowEntity> CalculateResults(SessionBaseEntity session, LeagueDbContext dbContext)
        {
            if (session == null || session.SessionResult == null)
                return null;

            //List<ScoredResultRowEntity> scoredResultRows = new List<ScoredResultRowEntity>();
            var scoredResult = ScoredResults.SingleOrDefault(x => x.Result.ResultId == session.SessionId);
            if (scoredResult == null)
            {
                scoredResult = new ScoredResultEntity()
                {
                    Result = session.SessionResult,
                    Scoring = this
                };
                ScoredResults.Add(scoredResult);
            }

            //dbContext.SaveChanges();

            var scoredResultRows = scoredResult.FinalResults;
            var resultRows = session.SessionResult.RawResults;

            IDictionary<int, int> basePoints = BasePoints.Split(' ').Select((x, i) => new { Item = int.Parse(x), Index = i }).ToDictionary(x => x.Index + 1, x => x.Item);
            IDictionary<int, int> bonusPoints = new Dictionary<int, int>();
            if (BonusPoints != "")
                bonusPoints = BonusPoints.Split(' ').Select(x => new { Item = int.Parse(x.Split(':').Last()), Index = int.Parse(x.Split(':').First().TrimStart(new char[] { 'p' })) }).ToDictionary(x => x.Index, x => x.Item);



            foreach (var resultRow in resultRows)
            {
                ScoredResultRowEntity scoredResultRow;
                if (scoredResultRows.Exists(x => x.ResultRow == resultRow)) {
                    scoredResultRow = scoredResultRows.Single(x => x.ResultRow == resultRow);
                }

                else {
                    scoredResultRow = new ScoredResultRowEntity()
                    {
                        ResultRow = resultRow,
                        //Scoring = this,
                    };
                    scoredResultRows.Add(scoredResultRow);
                }
                scoredResultRows.Add(scoredResultRow);

                scoredResultRow.RacePoints = basePoints.ContainsKey(resultRow.FinishPosition) ? basePoints[resultRow.FinishPosition] : 0;
                scoredResultRow.BonusPoints = bonusPoints.ContainsKey(resultRow.FinishPosition) ? bonusPoints[resultRow.FinishPosition] : 0;
                scoredResultRow.PenaltyPoints = 0;
                scoredResultRow.TotalPoints = scoredResultRow.RacePoints + scoredResultRow.BonusPoints - scoredResultRow.PenaltyPoints;
            }

            scoredResultRows = scoredResultRows.OrderBy(x => -x.TotalPoints).ToList();
            //scoredResultRows.Select((x, i) => new { Item = x, Index = i }).ToList().ForEach(x =>
            //{
            //    x.Item.FinalPosition = x.Index + 1;
            //    x.Item.FinalPositionChange = x.Item.ResultRow.StartPosition - x.Item.FinalPosition;
            //});
            for(int i = 0; i < scoredResultRows.Count(); i++)
            {
                var row = scoredResultRows.ElementAt(i);
                row.FinalPosition = i + 1;
                row.FinalPositionChange = row.ResultRow.StartPosition - row.FinalPosition;
            }

            var fastestLapRow = scoredResultRows.MinBy(x => x.ResultRow.FastestLapTime);
            scoredResult.FastestLap = fastestLapRow.ResultRow.FastestLapTime;
            scoredResult.FastestLapDriver = fastestLapRow.ResultRow.Member;

            var fastestAvgLapRow = scoredResultRows.MinBy(x => x.ResultRow.AvgLapTime);
            scoredResult.FastestAvgLap = fastestAvgLapRow.ResultRow.AvgLapTime;
            scoredResult.FastestAvgLapDriver = fastestAvgLapRow.ResultRow.Member;

            var fastestQualyLapRow = scoredResultRows.MinBy(x => x.ResultRow.QualifyingTime);
            scoredResult.FastestQualyLap = fastestQualyLapRow.ResultRow.QualifyingTime;
            scoredResult.FastestQualyLapDriver = fastestQualyLapRow.ResultRow.Member;

            return scoredResultRows;
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            ScoredResults.ToList().ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }

    public static class ScoredResultExtensions
    {
        public static IEnumerable<StandingsRowEntity> AggregateResults<T>(this IEnumerable<T> source) where T : ScoredResultRowEntity
        {
            return new List<StandingsRowEntity>();
        }
    }

    public static class EnumExtenstions
    {
        public static T MaxBy<T, TSelect>(this IEnumerable<T> source, Func<T, TSelect> selector) where TSelect : IComparable
        {
            return source.Aggregate((x, y) => selector(x).CompareTo(selector(y)) >= 0 ? x : y);
        }

        public static T MinBy<T, TSelect>(this IEnumerable<T> source, Func<T, TSelect> selector) where TSelect : IComparable
        {
            return source.Aggregate((x, y) => selector(x).CompareTo(selector(y)) <= 0 ? x : y);
        }
    }
}
