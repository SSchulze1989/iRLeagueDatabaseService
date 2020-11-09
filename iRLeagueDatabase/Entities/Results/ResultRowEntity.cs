using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Results
{
    [Serializable]
    public class ResultRowEntity : MappableEntity
    {
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ResultRowId { get; set; }
        [NotMapped]
        public override object MappingId => ResultRowId;

        //[Key, ForeignKey(nameof(Result)), Column(Order = 1)]
        [ForeignKey(nameof(Result))]
        public long ResultId { get; set; }
        [Required]
        public virtual ResultEntity Result { get; set; }

        public SimSessionTypeEnum SimSessionType { get; set; }

        public DateTime? Date => Result?.Session?.Date;

        //public int FinalPosition { get; set; }

        public int StartPosition { get; set; }

        public int FinishPosition { get; set; }

        [ForeignKey(nameof(Member))]
        public long MemberId { get; set; }
        public virtual LeagueMemberEntity Member { get; set; }

        public int OldIRating { get; set; }
        public int NewIRating { get; set; }
        public int SeasonStartIRating { get; set; }
        public string License { get; set; }
        public double OldSafetyRating { get; set; }
        public double NewSafetyRating { get; set; }
        public int OldCpi { get; set; }
        public int NewCpi { get; set; }
        public int ClubId { get; set; }
        public string ClubName { get; set; }
        public int CarNumber { get; set; }
        public int ClassId { get; set; }
        public string Car { get; set; }
        public int CarId { get; set; }
        public string CarClass { get; set; }
        public int CompletedLaps { get; set; } 
        public double CompletedPct { get; set; }
        public int LeadLaps { get; set; }
        public int FastLapNr { get; set; }
        public int Incidents { get; set; }
        public RaceStatusEnum Status { get; set; }
        public DateTime? QualifyingTimeAt { get; set; }
        public long QualifyingTime { get; set; }
        public long Interval { get; set; }
        public long AvgLapTime { get; set; }
        public long FastestLapTime { get; set; }
        public int PositionChange { get; set; }
        public int Division { get; set; }
        public int OldLicenseLevel { get; set; }
        public int NewLicenseLevel { get; set; }

        // Additional lap data. Has to be retrieved from iracing from different endpoints than results
        // Not sure if this will get used here or if there will be an additional lap data set in the parent result.
        // Num... value should be default -1 for no value.
        public int NumPitStops { get; set; }
        public string PittedLaps { get; set; }
        public int NumOfftrackLaps { get; set; }
        public string OfftrackLaps { get; set; }
        public int NumContactLaps { get; set; }
        public string ContactLaps { get; set; }


        [InverseProperty(nameof(ScoredResultRowEntity.ResultRow))]
        public virtual List<ScoredResultRowEntity> ScoredResultRows { get; set; }

        public ResultRowEntity() { }

        public override void Delete(LeagueDbContext dbContext)
        {
            ScoredResultRows.ToList().ForEach(x => x.Delete(dbContext));
            if (Result != null)
            {
                Result.RequiresRecalculation = true;
            }
            base.Delete(dbContext);
        }
    }
}
