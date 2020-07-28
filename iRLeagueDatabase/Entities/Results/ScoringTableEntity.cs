using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Sessions;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoringTableEntity : Revision
    {
        [Key]
        public long ScoringTableId { get; set; }
        public override object MappingId => ScoringTableId;
        public string Name { get; set; }
        public int DropWeeks { get; set; }
        public int AverageRaceNr { get; set; }
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string ScoringFactors { get; set; }
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

        public StandingsEntity GetSeasonStandings()
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

            return GetSeasonStandings(session, allSessions.Count - DropWeeks);
        }

        public StandingsEntity GetSeasonStandings(SessionBaseEntity currentSession)
        {
            var allSessions = GetAllSessions();
            return GetSeasonStandings(currentSession, allSessions.Count - DropWeeks);
        }

        public StandingsEntity GetSeasonStandings(SessionBaseEntity currentSession, int maxRacesCount = -1)
        {
            if (currentSession == null)
                return null;

            if (maxRacesCount == -1)
                maxRacesCount = Sessions.Count() - maxRacesCount;

            var allScoredResults = Scorings.SelectMany(x => x.ScoredResults).ToList();
            var previousScoredResults = allScoredResults.Where(x => x.Result.Session.Date < currentSession.Date).ToList();

            if (Scorings != null && Scorings.Count > 0)
            {
                foreach (var msc in Scorings)
                {
                    previousScoredResults.AddRange(msc.ScoredResults.Where(x => x.Result.Session.Date < currentSession.Date));
                    allScoredResults.AddRange(msc.ScoredResults);
                }
            }

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
    }
}
