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
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string ScoringFactors { get; set; }
        public DropRacesOption DropRacesOption { get; set; }
        public virtual List<ScoringEntity> Scorings { get; set; }
        [NotMapped]
        public IEnumerable<SessionBaseEntity> Sessions => Scorings?.SelectMany(x => x.Sessions);

        public ScoringTableEntity()
        {

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

            if (Scorings != null && Scorings.Count > 0)
            {
                foreach (var msc in Scorings)
                {
                    allSessions.AddRange(msc.Sessions);
                }
            }

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
            if (currentSession == null)
                return null;

            if (maxRacesCount == -1)
                maxRacesCount = Sessions.Count() - maxRacesCount;

            if (ScoringKind == ScoringKindEnum.Team)
            {
                return GetSeasonTeamStandings(currentSession, dbContext, maxRacesCount);
            }

            var allScoredResults = Scorings?.SelectMany(x => x.ScoredResults).ToList();
            var previousScoredResults = allScoredResults.Where(x => x.Result.Session.Date < currentSession.Date).ToList();

            var currentResult = currentSession.SessionResult;
            var currentScoredResult = allScoredResults.SingleOrDefault(x => x.Result.Session == currentSession);

            StandingsEntity standings = new StandingsEntity()
            {
                ScoringTable = this,
            };

            var previousScoredRows = previousScoredResults.SelectMany(x => x.FinalResults).ToList();
            var previousStandingsRows = previousScoredRows.AggregateByDriver(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            previousStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            allScoredResults = previousScoredResults.ToList();
            allScoredResults.Add(currentScoredResult);
            var currentStandingsRows = allScoredResults.SelectMany(x => x.FinalResults).AggregateByDriver(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            currentStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            standings.StandingsRows = currentStandingsRows.Diff(previousStandingsRows).OrderBy(x => -x.TotalPoints).ToList();
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
                    .Include(x => x.ScoredResultRows).Load();
            }

            var currentResult = currentSession.SessionResult;
            var currentScoredResult = allScoredResults.SingleOrDefault(x => x.Result.Session == currentSession);
            dbContext.Entry(currentScoredResult).Collection(x => x.TeamResults).Query()
                .Include(x => x.Team)
                .Include(x => x.ScoredResultRows).Load();

            TeamStandingsEntity teamStandings = new TeamStandingsEntity()
            {
                ScoringTable = this,
            };

            var previousScoredRows = previousScoredResults.SelectMany(x => x.TeamResults).ToList();
            var previousStandingsRows = previousScoredRows.AggregateByTeam(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            previousStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            allScoredResults = previousScoredResults.ToList();
            allScoredResults.Add(currentScoredResult);
            var currentStandingsRows = allScoredResults.SelectMany(x => x.TeamResults).AggregateByTeam(maxRacesCount, true).OrderBy(x => -x.TotalPoints);
            currentStandingsRows.Select((value, index) => new { index, value }).ToList().ForEach(x => x.value.Position = x.index + 1);

            teamStandings.StandingsRows = currentStandingsRows.Diff(previousStandingsRows).OrderBy(x => -x.TotalPoints).ToList();
            teamStandings.StandingsRows.ForEach(x => x.ScoringTable = this);
            //teamStandings.Calculate();

            return teamStandings;
        }
    }

    public static partial class ScoredResultExtensions
    {
        public static IEnumerable<TeamStandingsRowEntity> AggregateByTeam<T>(this IEnumerable<T> source, int maxRacesCount = -1, bool canDropPenaltyRace = true) where T : ScoredTeamResultRowEntity
        {
            var teamStandingsRows = new List<TeamStandingsRowEntity>();
            var teams = source.Select(x => x.Team).Distinct();

            foreach (var team in teams)
            {
                var teamResultRows = source.Where(x => x.Team == team).OrderBy(x => x.Date).OrderBy(x => -x.TotalPoints).Take(maxRacesCount).ToList();
                var teamDriverResultRows = teamResultRows.SelectMany(x => x.ScoredResultRows);

                var teamDriverStandingsRows = teamDriverResultRows.AggregateByDriver();

                var teamStandingsRow = teamDriverResultRows.AggregateTeamDriverResults();
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
