using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Members;
using iRLeagueManager.Enums;
using System.IO;

namespace iRLeagueDatabase.Entities.Results
{
    [NotMapped]
    public class StandingsRowEntity : Revision
    {
        public ScoringTableEntity ScoringTable { get; set; }
        public ScoringEntity Scoring { get; set; }
        public override object MappingId => new long[] { Scoring.ScoringId, Member.MemberId };
        public int Position { get; set; }
        public int LastPosition { get; set; }
        //[ForeignKey(nameof(Member))]
        //public int MemberId { get; set; }
        public virtual LeagueMemberEntity Member { get; set; }
        public int ClassId { get; set; }
        public string CarClass { get; set; }
        public double RacePoints { get; set; }
        public double RacePointsChange { get; set; }
        public double PenaltyPoints { get; set; }
        public double PenaltyPointsChange { get; set; }
        public double TotalPoints { get; set; }
        public double TotalPointsChange { get; set; }
        public int Races { get; set; }
        public int RacesCounted { get; set; }
        public int DroppedResultCount { get; set; }
        public double CompletedLaps { get; set; }
        public double CompletedLapsChange { get; set; }
        public double LeadLaps { get; set; }
        public double LeadLapsChange { get; set; }
        public int FastestLaps { get; set; }
        public int FastestLapsChange { get; set; }
        public int PolePositions { get; set; }
        public int PolePositionsChange { get; set; }
        public int Wins { get; set; }
        public int WinsChange { get; set; }
        public int Top3 { get; set; }
        public int Top5 { get; set; }
        public int Top10 { get; set; }
        public double Incidents { get; set; }
        public double IncidentsChange { get; set; }
        public int PositionChange { get; set; }
        public virtual List<ScoredResultRowEntity> CountedResults { get; set; }
        public virtual List<ScoredResultRowEntity> DroppedResults { get; set; }
        public virtual TeamEntity Team { get; set; }

        public StandingsRowEntity()
        {
            CarClass = "";
            ClassId = 0;
            CompletedLaps = 0;
            CompletedLapsChange = 0;
            DroppedResultCount = 0;
            FastestLaps = 0;
            Incidents = 0;
            IncidentsChange = 0;
            LastPosition = 0;
            LeadLaps = 0;
            LeadLapsChange = 0;
            PenaltyPoints = 0;
            PenaltyPointsChange = 0;
            PolePositions = 0;
            Position = 0;
            PositionChange = 0;
            RacePoints = 0;
            RacePointsChange = 0;
            Races = 0;
            Top10 = 0;
            Top3 = 0;
            Top5 = 0;
            TotalPoints = 0;
            TotalPointsChange = 0;
            Wins = 0;
            CountedResults = new List<ScoredResultRowEntity>();
            DroppedResults = new List<ScoredResultRowEntity>();
        }

        public StandingsRowEntity AddRows(ScoredResultRowEntity resultRow, bool countStats = true,  bool countPoints = true)
        {
            return AddRows(new ScoredResultRowEntity[] { resultRow }, countStats, countPoints);
        }

        public StandingsRowEntity AddRows(IEnumerable<ScoredResultRowEntity> resultRows, bool countStats = true, bool countPoints = true)
        {
            foreach (var scoredResultRow in resultRows)
            {
                if (countStats)
                {
                    //if (Scoring != scoredResultRow.ScoredResult.Scoring)
                    //    throw new InvalidOperationException("Scoring Entities of combining Standingsrows do not match. Can not combine rows from different Scoring tables!");

                    this.CompletedLaps += scoredResultRow.ResultRow.CompletedLaps;
                    this.Incidents += scoredResultRow.ResultRow.Incidents;
                    this.PenaltyPoints += scoredResultRow.PenaltyPoints;
                    this.PolePositions += (scoredResultRow.ResultRow.StartPosition == 1) ? 1 : 0;
                    this.Top10 += (scoredResultRow.FinalPosition <= 10) ? 1 : 0;
                    this.Top5 += (scoredResultRow.FinalPosition <= 5) ? 1 : 0;
                    this.Top3 += (scoredResultRow.FinalPosition <= 3) ? 1 : 0;
                    this.Wins += (scoredResultRow.FinalPosition == 1) ? 1 : 0;
                    this.FastestLaps += (scoredResultRow.ScoredResult?.FastestLapDriver.MemberId == this.Member.MemberId) ? 1 : 0;
                    this.Races += 1;
                }
                if (countPoints)
                {
                    this.RacePoints += scoredResultRow.RacePoints+scoredResultRow.BonusPoints;
                    this.TotalPoints = RacePoints - PenaltyPoints;
                    this.RacesCounted += 1;
                    this.CountedResults.Add(scoredResultRow);
                }
                else
                {
                    this.DroppedResults.Add(scoredResultRow);
                }
            }

            return this;
        }

        public StandingsRowEntity Diff(StandingsRowEntity compare)
        {
            var source = this;

            if (compare == null)
                return source;

            if (source.Scoring != compare.Scoring)
                throw new InvalidOperationException("Scoring Entities of combining Standingsrows do not match. Can not combine rows from different Scoring tables!");

            StandingsRowEntity standingsRow = new StandingsRowEntity()
            {
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
                CountedResults = source.CountedResults,
                DroppedResults = source.DroppedResults,
                Team = source.Team
            };

            return standingsRow;
        }

        public override long GetLeagueId()
        {
            return ScoringTable.GetLeagueId();
        }
    }
}
