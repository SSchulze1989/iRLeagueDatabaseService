using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoredTeamResultRowEntity : LeagueMappableEntity
    {
        [Key, Column(Order = 0)]
        public long ScoredResultRowId { get; set; }
        //[Key, ForeignKey(nameof(ResultRow)), Column(Order = 0)]
        //[ForeignKey(nameof(ResultRow))]
        //public long ResultRowId { get; set; }
        ////[Key, ForeignKey(nameof(ResultRow)), Column(Order = 1)]
        ////public long ResultId { get; set; }
        //public virtual ResultRowEntity ResultRow { get; set; }
        [NotMapped]
        public override object MappingId => ScoredResultRowId;
        [ForeignKey(nameof(ScoredTeamResult)), Column(Order = 1)]
        public long ScoredResultId { get; set; }
        [ForeignKey(nameof(ScoredTeamResult)), Column(Order = 2)]
        public long ScoringId { get; set; }
        //public virtual ScoringEntity Scoring { get; set; }
        //[ForeignKey(nameof(ScoredResult))]
        //public long ScoredResultId { get; set; }
        [Required]
        public virtual ScoredTeamResultEntity ScoredTeamResult { get; set; }

        [ForeignKey(nameof(League))]
        public override long LeagueId { get; set; }
        public override LeagueEntity League { get; set; }

        [ForeignKey(nameof(Team))]
        public long TeamId { get; set; }
        [Required]
        public virtual TeamEntity Team { get; set; }
        public DateTime? Date { get; set; }
        public int ClassId { get; set; }
        public string CarClass { get; set; }
        public double RacePoints { get; set; }
        public double BonusPoints { get; set; }
        //public int PenaltyPoints { get => (AddPenalty != null) ? AddPenalty.PenaltyPoints : 0; set { } }
        public double PenaltyPoints { get; set; }
        //[InverseProperty(nameof(AddPenaltyEntity.ScoredResultRow))]
        //public virtual AddPenaltyEntity AddPenalty { get; set; }
        //[InverseProperty(nameof(ReviewPenaltyEntity.ScoredResultRow))]
        //public virtual List<ReviewPenaltyEntity> ReviewPenalties { get; set; }
        public virtual List<ScoredResultRowEntity> ScoredResultRows { get; set; }
        public int FinalPosition { get; set; }
        public int FinalPositionChange { get; set; }
        public double TotalPoints { get; set; }
        public long AvgLapTime { get; set; }
        public long FastestLapTime { get; set; }

        public override void Delete(LeagueDbContext dbContext)
        {
            //ScoredResultRows?.ToList().ForEach(x => x.Delete(dbContext));
            ScoredResultRows?.ToList().ForEach(x => x.ScoredTeamResultRows.Remove(this));
            ScoredResultRows?.Clear();
            base.Delete(dbContext);
        }

        public void RemoveAllRows()
        {
            ScoredResultRows?.Clear();
            RacePoints = 0;
            BonusPoints = 0;
            PenaltyPoints = 0;
            TotalPoints = 0;
        }

        public ScoredTeamResultRowEntity AddRows(IEnumerable<ScoredResultRowEntity> resultRows)
        {
            if (ScoredResultRows == null)
                ScoredResultRows = new List<ScoredResultRowEntity>();
            foreach (var resultRow in resultRows)
            {
                ScoredResultRows.Add(resultRow);
                RacePoints += resultRow.RacePoints;
                BonusPoints += resultRow.BonusPoints;
                PenaltyPoints += resultRow.PenaltyPoints;
                TotalPoints += resultRow.TotalPoints;
            }

            return this;
        }
    }
}
