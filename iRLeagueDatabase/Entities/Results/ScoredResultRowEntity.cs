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
        [Key, ForeignKey(nameof(ResultRow)), Column(Order = 0)]
        public long ResultRowId { get; set; }
        [Key, ForeignKey(nameof(ResultRow)), Column(Order = 1)]
        public long ResultId { get; set; }
        public virtual ResultRowEntity ResultRow { get; set; }
        [NotMapped]
        public override object MappingId => new { ScoringId, ResultRowId };

        [Key, ForeignKey(nameof(Scoring)), Column(Order =2)]
        public long ScoringId { get; set; }
        [Required]
        public virtual ScoringEntity Scoring { get; set; }
        public int RacePoints { get; set; }
        public int BonusPoints { get; set; }
        public int PenaltyPoints { get; set; }
        public int FinalPosition { get; set; }
        public int FinalPositionChange { get; set; }
        public int TotalPoints { get; set; }
    }
}
