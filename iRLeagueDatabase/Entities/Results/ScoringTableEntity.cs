using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoringTableEntity : Revision
    {
        [Key]
        public long ScoringTableId { get; set; }
        public ScoringKindEnum ScoringKind { get; set; }
        public override object MappingId => ScoringTableId;
        public string Name { get; set; }
        public int DropWeeks { get; set; }
        public int AverageRaceNr { get; set; }
        [ForeignKey(nameof(Season))]
        public long SeasonId { get; set; }
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string ScoringFactors { get; set; }
        public DropRacesOption DropRacesOption { get; set; }
        public int ResultsPerRaceCount { get; set; }
        public virtual List<ScoringEntity> Scorings { get; set; }
        [NotMapped]
        public IEnumerable<SessionBaseEntity> Sessions => Scorings?.SelectMany(x => x.Sessions);

        public ScoringTableEntity()
        {

        }

        public override void Delete(LeagueDbContext dbContext)
        {
            Scorings?.Clear();
            base.Delete(dbContext);
        }

        public List<SessionBaseEntity> GetAllSessions()
        {
            if (Scorings != null && Scorings.Count > 0)
            {
                return Scorings.Where(x => x.Sessions != null).SelectMany(x => x.Sessions).ToList();
            }
            return Sessions.ToList();
        }

        public StandingsEntity GetSeasonStandings(LeagueDbContext dbContext)
        {
            var allSessions = GetAllSessions();

            //if (Scorings != null && Scorings.Count > 0)
            //{
            //    foreach (var msc in Scorings)
            //    {
            //        var addSessions = msc.Sessions.Except(allSessions);
            //        allSessions.AddRange(addSessions);
            //    }
            //}

            var session = allSessions.Where(x => x.SessionResult != null).OrderBy(x => x.Date).LastOrDefault();
            if (session == null)
                return null;

            return GetSeasonStandings(session, dbContext, allSessions.Count - DropWeeks);
        }

        private List<KeyValuePair<ScoringEntity, float>> GetScoringFactors()
        {
            //Get floating point values from ';' separated string - default to 1 if value can not be parsed.
            var scoringFactorValues = ScoringFactors.Split(';').Select(x => float.TryParse(x, out float res) ? res : (float)1);

            //Create list of key value pairs { Scoring, ScoringFactor} - default to 1 if factor is not in List.
            var pairs = Scorings
                .Select((x, i) => 
                    new KeyValuePair<ScoringEntity, float>(x, (i < scoringFactorValues.Count()) ? scoringFactorValues.ElementAt(i) : (float)1))
                .ToList();

            return pairs;
        }

        public StandingsEntity GetSeasonStandings(SessionBaseEntity currentSession, LeagueDbContext dbContext)
        {
            var allSessions = GetAllSessions();
            return GetSeasonStandings(currentSession, dbContext, allSessions.Count - DropWeeks);
        }

        public StandingsEntity GetSeasonStandings(SessionBaseEntity currentSession, LeagueDbContext dbContext, int maxRacesCount = -1)
        {
            StandingsEntity standings = new StandingsEntity()
            {
                ScoringTable = this,
                SessionId = (currentSession?.SessionId).GetValueOrDefault()
            };

            if (currentSession == null)
                return standings;

            if (maxRacesCount == -1)
                maxRacesCount = Sessions.Count() - DropWeeks;

            if (ScoringKind == ScoringKindEnum.Team)
            {
                return GetSeasonTeamStandings(currentSession, dbContext, maxRacesCount);
            }

            var allScoredResults = Scorings?.SelectMany(x => x.ScoredResults).ToList();
            var previousScoredResults = allScoredResults.Where(x => x.Result.Session.Date < currentSession.Date).ToList();

            var currentResult = currentSession.SessionResult;
            var currentScoredResult = allScoredResults.SingleOrDefault(x => x.Result.Session == currentSession);

            var previousScoredRows = previousScoredResults.SelectMany(x => x.FinalResults).ToList();
            var previousStandingsRows = previousScoredRows
                .AggregateByDriver(maxRacesCount, true)
                .OrderBy(x => -x.TotalPoints)
                .ThenBy(x => x.PenaltyPoints)
                .ThenBy(x => -x.Wins);
            previousStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            allScoredResults = previousScoredResults.ToList();
            allScoredResults.Add(currentScoredResult);
            var currentStandingsRows = allScoredResults
                .SelectMany(x => x.FinalResults)
                .AggregateByDriver(maxRacesCount, true)
                .OrderBy(x => -x.TotalPoints)
                .ThenBy(x => x.PenaltyPoints)
                .ThenBy(x => -x.Wins);
            currentStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            standings.StandingsRows = currentStandingsRows
                .Diff(previousStandingsRows)
                .OrderBy(x => -x.TotalPoints)
                .ThenBy(x => x.PenaltyPoints)
                .ThenBy(x => -x.Wins)
                .ToList();
            standings.StandingsRows.ForEach(x => x.ScoringTable = this);
            standings.Calculate();

            return standings;
        }

        public TeamStandingsEntity GetSeasonTeamStandings(SessionBaseEntity currentSession, LeagueDbContext dbContext, int maxRacesCount = -1)
        {
            if (currentSession == null)
                return null;

            if (maxRacesCount == -1)
                maxRacesCount = Sessions.Count() - maxRacesCount;

            var allScoredResults = Scorings?.SelectMany(x => x.ScoredResults).OfType<ScoredTeamResultEntity>().ToList();
            var previousScoredResults = allScoredResults.Where(x => x.Result.Session.Date < currentSession.Date).ToList();

            foreach(var scoredTeamResult in previousScoredResults)
            {
                dbContext.Entry(scoredTeamResult).Collection(x => x.TeamResults).Query()
                    .Include(x => x.Team)
                    .Include(x => x.ScoredResultRows.Select(y => y.ScoredResult)).Load();
            }

            var currentResult = currentSession.SessionResult;
            var currentScoredResult = allScoredResults.SingleOrDefault(x => x.Result.Session == currentSession);
            dbContext.Entry(currentScoredResult).Collection(x => x.TeamResults).Query()
                .Include(x => x.Team)
                .Include(x => x.ScoredResultRows.Select(y => y.ScoredResult)).Load();

            TeamStandingsEntity teamStandings = new TeamStandingsEntity()
            {
                ScoringTable = this,
                SessionId = currentSession.SessionId
            };

            var previousScoredRows = previousScoredResults.SelectMany(x => x.TeamResults).ToList();
            IEnumerable<TeamStandingsRowEntity> previousStandingsRows;
            IEnumerable<TeamStandingsRowEntity> currentStandingsRows;

            if (DropRacesOption == DropRacesOption.PerDriverResults)
            {

                var allScoredDriverResults = Scorings?.SelectMany(x => x.GetResultsFromSource()).ToList();

                var previousScoredDriverResults = allScoredDriverResults.Where(x => x.Result.Session.Date < currentSession.Date).ToList();

                var currentDriverResult = currentSession.SessionResult;
                var currentScoredDriverResult = allScoredDriverResults.SingleOrDefault(x => x.Result.Session == currentSession);
            
                var previousScoredDriverRows = previousScoredDriverResults.SelectMany(x => x.FinalResults);
                previousStandingsRows = previousScoredRows.AggregateByTeam(maxRacesCount, true, DropRacesOption, ResultsPerRaceCount, previousScoredDriverRows)
                    .OrderBy(x => -x.TotalPoints);

                allScoredDriverResults = previousScoredDriverResults.ToList();
                allScoredDriverResults.Add(currentScoredDriverResult);
                allScoredResults = previousScoredResults.ToList();
                allScoredResults.Add(currentScoredResult);

                var allScoredDriverRows = allScoredDriverResults.SelectMany(x => x.FinalResults);
                currentStandingsRows = allScoredResults.SelectMany(x => x.TeamResults)
                    .AggregateByTeam(maxRacesCount, true, DropRacesOption, ResultsPerRaceCount, allScoredDriverRows)
                    .OrderBy(x => -x.TotalPoints)
                    .ThenBy(x => x.PenaltyPoints)
                    .ThenBy(x => x.Wins);
            }
            else
            {
                previousStandingsRows = previousScoredRows.AggregateByTeam(maxRacesCount, true, DropRacesOption, ResultsPerRaceCount).OrderBy(x => -x.TotalPoints);

                allScoredResults = previousScoredResults.ToList();
                allScoredResults.Add(currentScoredResult);

                currentStandingsRows = allScoredResults.SelectMany(x => x.TeamResults).AggregateByTeam(maxRacesCount, true, DropRacesOption, ResultsPerRaceCount).OrderBy(x => -x.TotalPoints);
            }

            previousStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);
            currentStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            teamStandings.StandingsRows = currentStandingsRows
                .Diff(previousStandingsRows)
                .OrderBy(x => -x.TotalPoints)
                .ThenBy(x => x.PenaltyPoints)
                .ThenBy(x => -x.Wins)
                .ToList();
            //teamStandings.StandingsRows = currentStandingsRows.OrderBy(x => -x.TotalPoints).Cast<StandingsRowEntity>().ToList();
            teamStandings.StandingsRows.ForEach(x => x.ScoringTable = this);
            //teamStandings.Calculate();

            return teamStandings;
        }

        private IEnumerable<ScoredResultRowEntity> GetDroppedRaces(IEnumerable<ScoredResultRowEntity> source, int maxRacesCount = 0, bool canDropPenaltyRace = true)
        {
            source = source.OrderBy(x => x.ResultRow.Date).OrderBy(x => -x.TotalPoints);

            if (!canDropPenaltyRace)
            {
                source = source.OrderBy(x => !(x.PenaltyPoints != 0));
            }

            return source.Skip(maxRacesCount);
        }
    }

    public static partial class ScoredResultExtensions
    {
        public static IEnumerable<TeamStandingsRowEntity> AggregateByTeam<T>(this IEnumerable<T> source, int maxRacesCount = -1, bool canDropPenaltyRace = true, 
            DropRacesOption dropRacesOption = DropRacesOption.PerTeamResults, int resultsPerRace = 0, IEnumerable<ScoredResultRowEntity> allScoredResultRows = null) where T : ScoredTeamResultRowEntity
        {
            var teamStandingsRows = new List<TeamStandingsRowEntity>();
            var teams = source.Select(x => x.Team).Distinct();

            foreach (var team in teams)
            {
                
                IEnumerable<T> teamResultRows = source.Where(x => x.Team == team).OrderBy(x => x.Date).OrderBy(x => -x.TotalPoints);
                if (dropRacesOption == DropRacesOption.PerTeamResults)
                    teamResultRows = teamResultRows.Take(maxRacesCount);

                var teamDriverResultRows = teamResultRows.SelectMany(x => x.ScoredResultRows);
                if (dropRacesOption == DropRacesOption.PerDriverResults)
                {
                    if (allScoredResultRows != null)
                        teamDriverResultRows = allScoredResultRows.Where(x => x.TeamId == team.TeamId);

                    List<ScoredResultRowEntity> excludeResultRows = new List<ScoredResultRowEntity>();
                    var drivers= teamDriverResultRows.GroupBy(x => x.ResultRow.Member);
                    foreach (var driver in drivers)
                    {
                        // #### Hotfix for interval not being defined fix to positiv or negative values! This needs to be changed when final implementation is done!
                        var driverResultRows = driver.OrderBy(x => x.ResultRow.Date).OrderBy(x => Math.Abs(x.ResultRow.Interval)).OrderBy(x => -x.TotalPoints);
                        // ####

                        if (!canDropPenaltyRace)
                        {
                            driverResultRows = driverResultRows.OrderBy(x => !(x.PenaltyPoints != 0));
                        }

                        excludeResultRows.AddRange(driverResultRows.Skip(maxRacesCount));
                        //excludeResultRows.AddRange(driverResultRows.Where(x => x.TotalPoints < 0));
                    }
                    teamDriverResultRows = teamDriverResultRows.Except(excludeResultRows);

                    var groupedDriverResultRows = teamDriverResultRows.GroupBy(x => x.ScoredResult);
                    excludeResultRows = new List<ScoredResultRowEntity>();
                    
                    foreach (var result in groupedDriverResultRows)
                    {
                        var dropTeamResults = result.OrderBy(x => -x.TotalPoints).Skip(resultsPerRace);
                        excludeResultRows.AddRange(dropTeamResults);
                    }
                    
                    teamDriverResultRows = teamDriverResultRows.Except(excludeResultRows);
                }

                var teamDriverStandingsRows = teamDriverResultRows.AggregateByDriver(maxRacesCount: 100);

                var teamStandingsRow = teamDriverResultRows.AggregateTeamDriverResults();
                teamStandingsRow.DriverStandingsRows = teamDriverStandingsRows.ToList();
                teamStandingsRow.Team = team;
                teamStandingsRows.Add(teamStandingsRow);
            }

            return teamStandingsRows;
        }

        public static TeamStandingsRowEntity AggregateTeamDriverResults<T>(this IEnumerable<T> source) where T : ScoredResultRowEntity
        {
            source = source.OrderBy(x => x.ResultRow.Date).OrderBy(x => -x.TotalPoints);

            var standingsRow = new TeamStandingsRowEntity
            {
                //Scoring = source.First().ScoredResult.Scoring,
                Member = source.First().ResultRow.Member,
                ClassId = source.Last().ResultRow.ClassId,
                CarClass = source.Last().ResultRow.CarClass
            };

            standingsRow.AddRows(source, countPoints: true);

            return standingsRow;
        }
    }
}
