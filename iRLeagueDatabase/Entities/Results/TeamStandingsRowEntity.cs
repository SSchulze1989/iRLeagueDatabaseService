using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Results
{
    public class TeamStandingsRowEntity : StandingsRowEntity
    {
        public TeamEntity Team { get; set; }
        public virtual List<StandingsRowEntity> DriverStandingsRows { get; set; }

        public TeamStandingsRowEntity Diff(TeamStandingsRowEntity compare)
        {
            var source = this;

            if (compare == null)
                return source;

            if (source.Scoring != compare.Scoring)
                throw new InvalidOperationException("Scoring Entities of combining Standingsrows do not match. Can not combine rows from different Scoring tables!");

            var standingsRow = new TeamStandingsRowEntity()
            {
                Team = source.Team,
                DriverStandingsRows = source.DriverStandingsRows.Diff(compare.DriverStandingsRows).ToList(),
                Scoring = source.Scoring,
                CarClass = source.CarClass,
                ClassId = source.ClassId,
                CompletedLaps = source.CompletedLaps,
                CompletedLapsChange = source.CompletedLaps - compare.CompletedLaps,
                Incidents = source.Incidents,
                IncidentsChange = source.Incidents - compare.Incidents,
                DroppedResultCount = source.DroppedResultCount,
                FastestLaps = source.FastestLaps,
                FastestLapsChange = source.FastestLaps - compare.FastestLaps,
                LastPosition = compare.Position,
                LeadLaps = source.LeadLaps,
                LeadLapsChange = source.LeadLaps - compare.LeadLaps,
                Member = source.Member,
                PenaltyPoints = source.PenaltyPoints,
                PenaltyPointsChange = source.PenaltyPoints - compare.PenaltyPoints,
                Position = source.Position,
                //PositionChange = source.Position - compare.Position,
                PositionChange = compare.Position - source.Position,
                PolePositions = source.PolePositions,
                PolePositionsChange = source.PolePositions - compare.PolePositions,
                RacePoints = source.RacePoints,
                RacePointsChange = source.RacePoints - compare.RacePoints,
                Races = source.Races,
                RacesCounted = source.RacesCounted,
                Top10 = source.Top10,
                Top3 = source.Top3,
                Top5 = source.Top5,
                TotalPoints = source.TotalPoints,
                TotalPointsChange = source.TotalPoints - compare.TotalPoints,
                Wins = source.Wins,
                WinsChange = source.Wins - compare.Wins,
                CountedResults = source.CountedResults
            };

            return standingsRow;
        }
    }
}
