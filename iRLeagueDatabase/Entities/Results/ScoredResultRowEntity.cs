using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoredResultRowEntity : MappableEntity
    {
        [Key, Column(Order = 0)]
        public long ScoredResultRowId { get; set; }
        //[Key, ForeignKey(nameof(ResultRow)), Column(Order = 0)]
        [ForeignKey(nameof(ResultRow))]
        public long ResultRowId { get; set; }
        //[Key, ForeignKey(nameof(ResultRow)), Column(Order = 1)]
        //public long ResultId { get; set; }
        public virtual ResultRowEntity ResultRow { get; set; }
        [NotMapped]
        public override object MappingId => ScoredResultRowId;
        [ForeignKey(nameof(ScoredResult)), Column(Order = 1)]
        public long ScoredResultId { get; set; }
        [ForeignKey(nameof(ScoredResult)), Column(Order = 2)]
        public long ScoringId { get; set; }
        //public virtual ScoringEntity Scoring { get; set; }
        //[ForeignKey(nameof(ScoredResult))]
        //public long ScoredResultId { get; set; }
        [Required]
        public virtual ScoredResultEntity ScoredResult { get; set; }
        public int RacePoints { get; set; }
        public int BonusPoints { get; set; }
        public int PenaltyPoints { get => (AddPenalty != null) ? AddPenalty.PenaltyPoints : 0; set { } }
        [InverseProperty(nameof(AddPenaltyEntity.ScoredResultRow))]
        public virtual AddPenaltyEntity AddPenalty {get; set;}
        public int FinalPosition { get; set; }
        public int FinalPositionChange { get; set; }
        public int TotalPoints { get; set; }
    }
}
