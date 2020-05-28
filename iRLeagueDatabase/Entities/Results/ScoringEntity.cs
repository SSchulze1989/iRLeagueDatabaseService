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

        public IEnumerable<StandingsRowEntity> GetSeasonStandings()
        {
            var session = Sessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();
            if (session == null)
                return null;

            return GetSeasonStandings(session.SessionId);
        }

        public IEnumerable<StandingsRowEntity> GetSeasonStandings(long sessionId)
        {
            return null;

            //var currentSession = Sessions.SingleOrDefault(x => x.SessionId == sessionId);
            
            //if (currentSession == null)
            //    currentSession = Sessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();
            
            //if (currentSession == null)
            //    return null;

            //var previousSessions = Sessions.Where(x => x.Date < currentSession.Date).OrderBy(x => x.Date);
            //var previousResults = previousSessions.Where(x => x.SessionResult != null).Select(x => x.SessionResult);
            //var previousScoredResults = previousResults.Select(x =>
            //{
            //    return x;
            //});

            //List<StandingsRowEntity> standings = new List<StandingsRowEntity>();

            //// Get unique Standings row per driver
            //var drivers = previousSessions.SelectMany(x => x.SessionResult.RawResults).Select(x => x.Member).Concat(currentSession.SessionResult.RawResults.Select(x => x.Member)).Distinct();
            //foreach(var driver in drivers)
            //{
            //    var standingsRow = new StandingsRowEntity()
            //    {
            //        CarClass = "",
            //        ClassId = 0,
            //        CompletedLaps = 0,
            //        CompletedLapsChange = 0,
            //        DroppedResults = 0,
            //        FastestLaps = 0,
            //        Incidents = 0,
            //        IncidentsChange = 0,
            //        LastPosition = 0,
            //        LeadLaps = 0,
            //        LeadLapsChange = 0,
            //        Member = driver,
            //        PenaltyPoints = 0,
            //        PenaltyPointsChange = 0,
            //        PolePositions = 0,
            //        Position = 0,
            //        PositionChange = 0,
            //        RacePoints = 0,
            //        RacePointsChange = 0,
            //        Races = 0,
            //        Top10 = 0,
            //        Top3 = 0,
            //        Top5 = 0,
            //        TotalPoints = 0,
            //        TotalPointsChange = 0,
            //        Wins = 0
            //    };
            //    standings.Add(standingsRow);
                

            //    var driverPreviousResultRows = ScoredResultRows
            //        .Where(x => x.ResultRow.Member.MemberId == driver.MemberId && previousResults.Contains(x.ResultRow.Result))
            //        .OrderBy(x => -x.TotalPoints).ToArray();
            //    var driverCurrentResultRow = ScoredResultRows.SingleOrDefault(x => x.ResultRow.Member.MemberId == driver.MemberId && currentSession.SessionResult == x.ResultRow.Result);

            //    var driverDropRaces = driverPreviousResultRows.Count() - (Sessions.Count() - DropWeeks) + 1;
            //    bool dropCurrentRace = false;

            //    var driverScoredResultRows = driverPreviousResultRows;

            //    if (driverDropRaces > 0)
            //    {
            //        if (driverPreviousResultRows[-driverDropRaces].TotalPoints >= driverCurrentResultRow.TotalPoints)
            //        {
            //            dropCurrentRace = true;
            //            driverDropRaces--;
            //        }
            //        driverScoredResultRows = driverScoredResultRows.Take(driverScoredResultRows.Count() - driverDropRaces).ToArray();
            //    }

            //    foreach(var scoredResult in driverScoredResultRows)
            //    {
            //        standingsRow.CompletedLaps += scoredResult.ResultRow.CompletedLaps;
            //        standingsRow.Incidents += scoredResult.ResultRow.Incidents;
            //        standingsRow.PenaltyPoints += scoredResult.PenaltyPoints;
            //        standingsRow.PolePositions += (scoredResult.ResultRow.StartPosition == 1) ? 1 : 0;
            //        standingsRow.RacePoints += scoredResult.RacePoints;
            //        standingsRow.Top10 += (scoredResult.FinalPosition <= 10) ? 1 : 0;
            //        standingsRow.Top5 += (scoredResult.FinalPosition <= 5) ? 1 : 0;
            //        standingsRow.Top3 += (scoredResult.FinalPosition <= 3) ? 1 : 0;
            //        standingsRow.TotalPoints += scoredResult.TotalPoints;
            //        standingsRow.Wins += (scoredResult.FinalPosition == 1) ? 1 : 0;
            //    }

            //    if (!dropCurrentRace)
            //    {
            //        standingsRow.CompletedLapsChange = standingsRow.CompletedLaps + driverCurrentResultRow.ResultRow.CompletedLaps;
            //        standingsRow.CompletedLaps += driverCurrentResultRow.ResultRow.CompletedLaps;
            //        standingsRow.IncidentsChange = standingsRow.Incidents + driverCurrentResultRow.ResultRow.Incidents;
            //        standingsRow.Incidents += driverCurrentResultRow.ResultRow.Incidents;
            //        standingsRow.PenaltyPointsChange = standingsRow.PenaltyPoints + driverCurrentResultRow.PenaltyPoints;
            //        standingsRow.PenaltyPoints += driverCurrentResultRow.PenaltyPoints;
            //        standingsRow.PolePositions += (driverCurrentResultRow.ResultRow.StartPosition == 1) ? 1 : 0;
            //        standingsRow.RacePointsChange = standingsRow.RacePoints + driverCurrentResultRow.RacePoints;
            //        standingsRow.RacePoints += driverCurrentResultRow.RacePoints;
            //        standingsRow.Top10 += (driverCurrentResultRow.FinalPosition <= 10) ? 1 : 0;
            //        standingsRow.Top5 += (driverCurrentResultRow.FinalPosition <= 5) ? 1 : 0;
            //        standingsRow.Top3 += (driverCurrentResultRow.FinalPosition <= 3) ? 1 : 0;
            //        standingsRow.TotalPointsChange = standingsRow.TotalPoints + driverCurrentResultRow.TotalPoints;
            //        standingsRow.TotalPoints += driverCurrentResultRow.TotalPoints;
            //        standingsRow.Wins += (driverCurrentResultRow.FinalPosition == 1) ? 1 : 0;
            //    }
            //}
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

            return scoredResultRows;
        }
    }
}
