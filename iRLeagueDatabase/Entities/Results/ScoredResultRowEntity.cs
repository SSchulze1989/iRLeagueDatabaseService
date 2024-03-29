﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Statistics;
using System.Data.Entity;
using System.CodeDom;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoredResultRowEntity : MappableEntity, IResultRow
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

        [ForeignKey(nameof(Team))]
        public long? TeamId { get; set; }
        public virtual TeamEntity Team { get; set; }

        public double RacePoints { get; set; }
        public double BonusPoints { get; set; }
        //public int PenaltyPoints { get => (AddPenalty != null) ? AddPenalty.PenaltyPoints : 0; set { } }
        public double PenaltyPoints { get; set; }
        public long PenaltyTime { get; set; }
        [InverseProperty(nameof(AddPenaltyEntity.ScoredResultRow))]
        public virtual AddPenaltyEntity AddPenalty { get; set; }
        [InverseProperty(nameof(ReviewPenaltyEntity.ScoredResultRow))]
        public virtual List<ReviewPenaltyEntity> ReviewPenalties { get; set; }
        //[ForeignKey(nameof(TeamResultRow))]
        //public long? TeamResultRowId { get; set; }
        //public virtual ScoredTeamResultRowEntity TeamResultRow { get; set; }
        public int FinalPosition { get; set; }
        public double FinalPositionChange { get; set; }
        public double TotalPoints { get; set; }
        public bool PointsEligible { get; set; }

        public SimSessionTypeEnum SimSessionType => ((IResultRow)ResultRow).SimSessionType;

        public DateTime? Date => ((IResultRow)ResultRow).Date;

        public double StartPosition => ((IResultRow)ResultRow).StartPosition;

        public double FinishPosition => ((IResultRow)ResultRow).FinishPosition;

        public long MemberId => ((IResultRow)ResultRow).MemberId;

        public LeagueMemberEntity Member => ((IResultRow)ResultRow).Member;

        public int OldIRating => ((IResultRow)ResultRow).OldIRating;

        public int NewIRating => ((IResultRow)ResultRow).NewIRating;

        public int SeasonStartIRating => ((IResultRow)ResultRow).SeasonStartIRating;

        public string License => ((IResultRow)ResultRow).License;

        public double OldSafetyRating => ((IResultRow)ResultRow).OldSafetyRating;

        public double NewSafetyRating => ((IResultRow)ResultRow).NewSafetyRating;

        public int OldCpi => ((IResultRow)ResultRow).OldCpi;

        public int NewCpi => ((IResultRow)ResultRow).NewCpi;

        public int ClubId => ((IResultRow)ResultRow).ClubId;

        public string ClubName => ((IResultRow)ResultRow).ClubName;

        public int CarNumber => ((IResultRow)ResultRow).CarNumber;

        public int ClassId => ((IResultRow)ResultRow).ClassId;

        public string Car => ((IResultRow)ResultRow).Car;

        public int CarId => ((IResultRow)ResultRow).CarId;

        public string CarClass => ((IResultRow)ResultRow).CarClass;

        public double CompletedLaps => ((IResultRow)ResultRow).CompletedLaps;

        public double CompletedPct => ((IResultRow)ResultRow).CompletedPct;

        public double LeadLaps => ((IResultRow)ResultRow).LeadLaps;

        public int FastLapNr => ((IResultRow)ResultRow).FastLapNr;

        public double Incidents => ((IResultRow)ResultRow).Incidents;

        public RaceStatusEnum Status => ((IResultRow)ResultRow).Status;

        public DateTime? QualifyingTimeAt => ((IResultRow)ResultRow).QualifyingTimeAt;

        public long QualifyingTime => ((IResultRow)ResultRow).QualifyingTime;

        public long Interval => ((IResultRow)ResultRow).Interval + PenaltyTime;

        public long AvgLapTime => ((IResultRow)ResultRow).AvgLapTime;

        public long FastestLapTime => ((IResultRow)ResultRow).FastestLapTime;

        public double PositionChange => ((IResultRow)ResultRow).PositionChange;

        public int Division => ((IResultRow)ResultRow).Division;

        public int OldLicenseLevel => ((IResultRow)ResultRow).OldLicenseLevel;

        public int NewLicenseLevel => ((IResultRow)ResultRow).NewLicenseLevel;

        public int NumPitStops => ((IResultRow)ResultRow).NumPitStops;

        public string PittedLaps => ((IResultRow)ResultRow).PittedLaps;

        public int NumOfftrackLaps => ((IResultRow)ResultRow).NumOfftrackLaps;

        public string OfftrackLaps => ((IResultRow)ResultRow).OfftrackLaps;

        public int NumContactLaps => ((IResultRow)ResultRow).NumContactLaps;

        public string ContactLaps => ((IResultRow)ResultRow).ContactLaps;

        public virtual List<ScoredTeamResultRowEntity> ScoredTeamResultRows { get; set; }

        public override void Delete(LeagueDbContext dbContext)
        {
            AddPenalty?.Delete(dbContext);
            ReviewPenalties?.ToList().ForEach(x => x.Delete(dbContext));
            dbContext.Set<DriverStatisticRowEntity>()
                .Where(x => x.LastResultRowId == ScoredResultRowId || x.FirstResultRowId == ScoredResultRowId)
                .Load();
            ScoredTeamResultRows?.ToList().ForEach(x => x.ScoredResultRows.Remove(this));
            ScoredTeamResultRows?.Clear();

            base.Delete(dbContext);
        }
    }
}
