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
        public virtual List<SessionBaseEntity> Sessions { get; set; }
        [ForeignKey(nameof(Season))]
        public long SeasonId { get; set; }
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string BasePoints { get; set; }
        public string BonusPoints { get; set; }
        public string IncPenaltyPoints { get; set; }
        public string MultiScoringFactors { get; set; }
        public virtual List<ScoringEntity> MultiScoringResults { get; set; }
        public virtual List<ScoredResultRowEntity> ScoredResultRows { get; set; }

        //public ScoringRuleBase Rule { get; set; }
        public ScoringEntity() { }

        public IEnumerable<ScoredResultRowEntity> CalculateResults(long sessionId)
        {
            if (!Sessions.Any(x => x.SessionId == sessionId))
                return null;

            var session = Sessions.SingleOrDefault(x => x.SessionId == sessionId);

            return CalculateResults(session);
        }

        public IEnumerable<ScoredResultRowEntity> CalculateResults(SessionBaseEntity session)
        {
            List<ScoredResultRowEntity> scoredResultRows = new List<ScoredResultRowEntity>();
            var resultRows = session.SessionResult?.RawResults;

            IDictionary<int, int> basePoints = BasePoints.Split(' ').Select((x, i) => new { Item = int.Parse(x), Index = i }).ToDictionary(x => x.Index + 1, x => x.Item);
            IDictionary<int, int> bonusPoints = BonusPoints.Split(' ').Select(x => new { Item = int.Parse(x.Split(':').Last()), Index = int.Parse(x.Split(':').First().TrimStart(new char[] { 'p' })) }).ToDictionary(x => x.Index, x => x.Item);

            foreach (var resultRow in resultRows)
            {
                ScoredResultRowEntity scoredResultRow;
                if (ScoredResultRows.Exists(x => x.ResultRow == resultRow)) {
                    scoredResultRow = ScoredResultRows.Single(x => x.ResultRow == resultRow);
                }

                else {
                    scoredResultRow = new ScoredResultRowEntity()
                    {
                        ResultRow = resultRow,
                        Scoring = this,
                    };
                    ScoredResultRows.Add(scoredResultRow);
                }
                scoredResultRows.Add(scoredResultRow);

                scoredResultRow.RacePoints = basePoints.ContainsKey(resultRow.FinishPosition) ? basePoints[resultRow.FinishPosition] : 0;
                scoredResultRow.BonusPoints = bonusPoints.ContainsKey(resultRow.FinishPosition) ? bonusPoints[resultRow.FinishPosition] : 0;
                scoredResultRow.PenaltyPoints = 0;
                scoredResultRow.TotalPoints = scoredResultRow.RacePoints + scoredResultRow.BonusPoints - scoredResultRow.PenaltyPoints;
            }

            scoredResultRows = ScoredResultRows.OrderBy(x => -x.TotalPoints).ToList();
            scoredResultRows.Select((x, i) => new { Item = x, Index = i }).ToList().ForEach(x =>
            {
                x.Item.FinalPosition = x.Index + 1;
                x.Item.FinalPositionChange = x.Item.ResultRow.StartPosition - x.Item.FinalPosition;
            });

            return scoredResultRows;
        }
    }
}
