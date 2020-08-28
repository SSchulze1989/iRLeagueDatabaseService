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
using iRLeagueDatabase.Entities.Reviews;

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
        public bool IsMultiScoring { get; set; }

        public List<SessionBaseEntity> sessions;
        public virtual List<SessionBaseEntity> Sessions
        {
            get
            {
                //if (ConnectedSchedule != null)
                //{
                //    sessions = ConnectedSchedule.Sessions;
                //}
                //else
                //{
                //    return sessions?.Distinct().ToList();
                //}
                return sessions;
            }
            set => sessions = value;
        }
        [NotMapped]
        public virtual IEnumerable<ResultEntity> Results => Sessions?.Select(x => x.SessionResult);

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

        /// <summary>
        /// Get all sessions, including sessions from Multiscoring
        /// </summary>
        /// <returns></returns>
        public List<SessionBaseEntity> GetAllSessions()
        {
            if (IsMultiScoring && MultiScoringResults != null && MultiScoringResults.Count > 0)
            {
                return MultiScoringResults.Where(x => x.Sessions != null).SelectMany(x => x.Sessions).ToList();
            }
            return Sessions;
        }

        public StandingsEntity GetSeasonStandings()
        {
            var allSessions = GetAllSessions();

            if (MultiScoringResults != null && MultiScoringResults.Count > 0)
            {
                foreach(var msc in MultiScoringResults)
                {
                    allSessions.AddRange(msc.Sessions);
                }
            }

            var session = allSessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();
            if (session == null)
                return null;

            return GetSeasonStandings(session, allSessions.Count - DropWeeks);
        }

        public StandingsEntity GetSeasonStandings(SessionBaseEntity currentSession)
        {
            var allSessions = GetAllSessions();
            return GetSeasonStandings(currentSession, allSessions.Count - DropWeeks);
        }

        public StandingsEntity GetSeasonStandings(SessionBaseEntity currentSession, int maxRacesCount = -1)
        {

            //var currentSession = Sessions.SingleOrDefault(x => x.SessionId == sessionId);

            //if (currentSession == null)
            //    currentSession = Sessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();

            if (currentSession == null)
                return null;

            if (maxRacesCount == -1)
                maxRacesCount = Sessions.Count - maxRacesCount;

            //var previousSessions = Sessions.Where(x => x.Date < currentSession.Date);
            //var previousResults = previousSessions.Where(x => x.SessionResult != null).Select(x => x.SessionResult);
            
            var allScoredResults = ScoredResults.ToList();
            var previousScoredResults = allScoredResults.Where(x => x.Result.Session.Date < currentSession.Date).ToList();

            if (MultiScoringResults != null && MultiScoringResults.Count > 0)
            {
                foreach (var msc in MultiScoringResults)
                {
                    previousScoredResults.AddRange(msc.ScoredResults.Where(x => x.Result?.Session.Date < currentSession.Date));
                    allScoredResults.AddRange(msc.ScoredResults);
                }
            }

            var currentResult = currentSession.SessionResult;
            var currentScoredResult = allScoredResults.SingleOrDefault(x => x.Result.Session == currentSession);

            StandingsEntity standings = new StandingsEntity()
            {
                Scoring = this,
            };

            var previousScoredRows = previousScoredResults.SelectMany(x => x.FinalResults).ToList();
            //previousScoredRows.ForEach(x => x.ResultRow = x.ResultRow);
            var previousStandingsRows = previousScoredRows.AggregateByDriver(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            //var previousStandingsRows = previousScoredResults.SelectMany(x => x.FinalResults).AggregateByDriver(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            previousStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            allScoredResults = previousScoredResults.ToList();
            allScoredResults.Add(currentScoredResult);
            var currentStandingsRows = allScoredResults.SelectMany(x => x.FinalResults).AggregateByDriver(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            currentStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            standings.StandingsRows = currentStandingsRows.Diff(previousStandingsRows).OrderBy(x => -x.TotalPoints).ToList();
            standings.StandingsRows.ForEach(x => x.Scoring = this);
            standings.Calculate();

            return standings;

            // Get unique Standings row per driver

            //foreach (var driver in drivers)
            //{
            //    var standingsRow = new StandingsRowEntity
            //    {
            //        Member = driver
            //    };
            //    previousStandingsRows.Add(standingsRow);


            //    var driverPreviousResults = ScoredResults
            //        .Where(x => x.Result.DriverList.Contains(driver));
            //    var driverPreviousResultRows = driverPreviousResults
            //        .Select(x => x.FinalResults.SingleOrDefault(y => y.ResultRow.Member == driver))
            //        .OrderBy(x => -x.TotalPoints).ToArray();
            //    var driverCurrentResultRow = currentScoredResult?.FinalResults.SingleOrDefault(x => x.ResultRow.Member == driver);

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

            //    foreach (var scoredResult in driverPreviousResults)
            //    {
            //        standingsRow.FastestLaps += (scoredResult.FastestLapDriver == driver) ? 1 : 0;
            //    }

            //    foreach (var scoredResultRow in driverScoredResultRows)
            //    {
            //        standingsRow.CompletedLaps += scoredResultRow.ResultRow.CompletedLaps;
            //        standingsRow.Incidents += scoredResultRow.ResultRow.Incidents;
            //        standingsRow.PenaltyPoints += scoredResultRow.PenaltyPoints;
            //        standingsRow.PolePositions += (scoredResultRow.ResultRow.StartPosition == 1) ? 1 : 0;
            //        standingsRow.RacePoints += scoredResultRow.RacePoints;
            //        standingsRow.Top10 += (scoredResultRow.FinalPosition <= 10) ? 1 : 0;
            //        standingsRow.Top5 += (scoredResultRow.FinalPosition <= 5) ? 1 : 0;
            //        standingsRow.Top3 += (scoredResultRow.FinalPosition <= 3) ? 1 : 0;
            //        standingsRow.TotalPoints += scoredResultRow.TotalPoints;
            //        standingsRow.Wins += (scoredResultRow.FinalPosition == 1) ? 1 : 0;
            //    }

            //    // Correct Total points for not dropped Penalties:
            //    standingsRow.PenaltyPoints = driverPreviousResultRows.Sum(x => x.PenaltyPoints);

            //    StandingsRowEntity currentStandingsRow;
            //    if (!dropCurrentRace)
            //    {
            //        currentStandingsRow = new StandingsRowEntity
            //        {
            //            Member = driver,
            //            CarClass = driverCurrentResultRow.ResultRow.CarClass,
            //            ClassId = driverCurrentResultRow.ResultRow.ClassId
            //        };
            //        currentStandingsRow.CompletedLapsChange = driverCurrentResultRow.ResultRow.CompletedLaps;
            //        currentStandingsRow.CompletedLaps += driverCurrentResultRow.ResultRow.CompletedLaps;
            //        currentStandingsRow.IncidentsChange = driverCurrentResultRow.ResultRow.Incidents;
            //        currentStandingsRow.Incidents += driverCurrentResultRow.ResultRow.Incidents;
            //        currentStandingsRow.PenaltyPointsChange = driverCurrentResultRow.PenaltyPoints;
            //        currentStandingsRow.PenaltyPoints += driverCurrentResultRow.PenaltyPoints;
            //        currentStandingsRow.PolePositions += (driverCurrentResultRow.ResultRow.StartPosition == 1) ? 1 : 0;
            //        currentStandingsRow.RacePointsChange = driverCurrentResultRow.RacePoints;
            //        currentStandingsRow.RacePoints += driverCurrentResultRow.RacePoints;
            //        currentStandingsRow.Top10 += (driverCurrentResultRow.FinalPosition <= 10) ? 1 : 0;
            //        currentStandingsRow.Top5 += (driverCurrentResultRow.FinalPosition <= 5) ? 1 : 0;
            //        currentStandingsRow.Top3 += (driverCurrentResultRow.FinalPosition <= 3) ? 1 : 0;
            //        currentStandingsRow.TotalPointsChange = currentStandingsRow.TotalPoints + driverCurrentResultRow.TotalPoints;
            //        currentStandingsRow.TotalPoints += driverCurrentResultRow.TotalPoints;
            //        currentStandingsRow.Wins += (driverCurrentResultRow.FinalPosition == 1) ? 1 : 0;
            //    }
            //    else
            //    {
            //        currentStandingsRow = standingsRow;
            //    }
            //    currentStandingsRows.Add(currentStandingsRow);
            //}

            //previousStandingsRows = previousStandingsRows.OrderBy(x => -x.TotalPoints).ToList();
        }

        public IEnumerable<ScoredResultRowEntity> CalculateResults(long sessionId, LeagueDbContext dbContext)
        {
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
                Sessions = ConnectedSchedule.Sessions;
            }
        }

        public IEnumerable<ScoredResultRowEntity> CalculateResults(SessionBaseEntity session, LeagueDbContext dbContext)
        {
            UpdateSessionList();
            if (session == null || session.SessionResult == null)
                return null;

            //List<ScoredResultRowEntity> scoredResultRows = new List<ScoredResultRowEntity>();
            var scoredResult = ScoredResults.SingleOrDefault(x => x.ResultId == session.SessionId);
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

            var reviewVotes = new List<AcceptedReviewVoteEntity>();

            if (session.Reviews != null)
            {
                foreach(var review in session.Reviews)
                {
                    if (review.AcceptedReviewVotes != null && review.AcceptedReviewVotes.Count > 0)
                    {
                        reviewVotes.AddRange(review.AcceptedReviewVotes);
                    }
                }
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
                if (scoredResultRows.Exists(x => x.ResultRowId == resultRow.ResultRowId)) 
                {
                    scoredResultRow = scoredResultRows.Single(x => x.ResultRowId == resultRow.ResultRowId);
                }
                else 
                {
                    scoredResultRow = new ScoredResultRowEntity()
                    {
                        ResultRow = resultRow,
                        //Scoring = this,
                    };
                    scoredResultRows.Add(scoredResultRow);
                }

                var scoredResultRowReviewVotes = reviewVotes.Where(x => x.MemberAtFaultId == scoredResultRow.ResultRow.MemberId);
                if (scoredResultRow.ReviewPenalties != null)
                {
                    var removePenalty = scoredResultRow.ReviewPenalties.ToList();
                    foreach (var reviewVote in scoredResultRowReviewVotes)
                    {
                        var reviewPenalty = scoredResultRow.ReviewPenalties.SingleOrDefault(x => x.ReviewId == reviewVote.ReviewId);
                        if (reviewPenalty == null)
                        {
                            reviewPenalty = new ReviewPenaltyEntity()
                            {
                                Review = reviewVote.IncidentReview,
                                ScoredResultRow = scoredResultRow
                            };
                            scoredResultRow.ReviewPenalties.Add(reviewPenalty);
                        }
                        else
                        {
                            removePenalty.Remove(reviewPenalty);
                        }
                        reviewPenalty.PenaltyPoints = GetReviewPenaltyPoints(reviewVote.Vote);
                    }
                    removePenalty.ForEach(x => x.Delete(dbContext));
                    //dbContext.SaveChanges();
                }

                scoredResultRow.RacePoints = basePoints.ContainsKey(resultRow.FinishPosition) ? basePoints[resultRow.FinishPosition] : 0;
                scoredResultRow.BonusPoints = bonusPoints.ContainsKey(resultRow.FinishPosition) ? bonusPoints[resultRow.FinishPosition] : 0;
                scoredResultRow.PenaltyPoints = GetPenaltyPoints(scoredResultRow);
                scoredResultRow.TotalPoints = scoredResultRow.RacePoints + scoredResultRow.BonusPoints - scoredResultRow.PenaltyPoints;
            }

            //var droppedRows = scoredResultRows.Where(x => x.ResultRow.CompletedLaps == 0).ToList();
            //scoredResultRows = scoredResultRows.Except(droppedRows).ToList();
            //dbContext.Set<ScoredResultRowEntity>().RemoveRange(droppedRows.Where(x => dbContext.Entry(x).State != System.Data.Entity.EntityState.Detached));

            scoredResultRows = scoredResultRows.OrderBy(x => x.PenaltyPoints).OrderBy(x => -x.TotalPoints).ToList();
            //scoredResultRows.Select((x, i) => new { Item = x, Index = i }).ToList().ForEach(x =>
            //{
            //    x.Item.FinalPosition = x.Index + 1;
            //    x.Item.FinalPositionChange = x.Item.ResultRow.StartPosition - x.Item.FinalPosition;
            //});
            ScoredResultRowEntity previousRow = null;
            for(int i = 0; i < scoredResultRows.Count(); i++)
            {
                var row = scoredResultRows.ElementAt(i);
                if (previousRow != null && row.TotalPoints == previousRow.TotalPoints && row.PenaltyPoints == previousRow.PenaltyPoints)
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

        private int GetReviewPenaltyPoints(VoteEnum vote)
        {
            switch (vote)
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

        public override void Delete(LeagueDbContext dbContext)
        {
            ScoredResults.ToList().ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }

    public static class ScoredResultExtensions
    {
        public static IEnumerable<StandingsRowEntity> Diff<T>(this IEnumerable<T> source, IEnumerable<T> compare) where T : StandingsRowEntity
        {
            List<StandingsRowEntity> resultList = new List<StandingsRowEntity>();
            foreach (var row in source)
            {
                StandingsRowEntity resultRow;
                var compRow = compare.SingleOrDefault(x => x.Member.MemberId == row.Member.MemberId);
                if (compRow != null)
                {
                    resultRow = row.Diff(compRow);
                }
                else
                {
                    resultRow = row;
                }

                resultList.Add(resultRow);
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
                CarClass = source.Last().ResultRow.CarClass
            };

            standingsRow.AddRows(source.Skip(maxRacesCount).ToArray(), countPoints: false);
            standingsRow.AddRows(source.Take(maxRacesCount).ToArray(), countPoints: true);

            return standingsRow;
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
            return source.Aggregate((x, y) => selector(x).CompareTo(selector(y)) <= 0 ? x : y);
        }
    }
}
