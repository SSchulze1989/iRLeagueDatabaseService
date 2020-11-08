namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomResultsFilter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IRSimSessionDetailsEntities",
                c => new
                    {
                        ResultId = c.Long(nullable: false),
                        IRSubsessionId = c.Long(nullable: false),
                        IRSeasonId = c.Long(nullable: false),
                        IRSeasonName = c.Long(nullable: false),
                        IRSeasonYear = c.Int(nullable: false),
                        IRSeasonQuarter = c.Int(nullable: false),
                        IRRaceWeek = c.Int(nullable: false),
                        IRSessionId = c.Long(nullable: false),
                        LicenseCategory = c.Int(nullable: false),
                        SessionName = c.String(),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        CornersPerLap = c.Int(nullable: false),
                        KmDistPerLap = c.Double(nullable: false),
                        MaxWeeks = c.Int(nullable: false),
                        EventStrengthOfField = c.Int(nullable: false),
                        EventAverageLap = c.Long(nullable: false),
                        EventLapsComplete = c.Int(nullable: false),
                        NumCautions = c.Int(nullable: false),
                        NumCautionLaps = c.Int(nullable: false),
                        NumLeadChanges = c.Int(nullable: false),
                        TimeOfDay = c.Int(nullable: false),
                        DamageModel = c.Int(nullable: false),
                        IRTrackId = c.Int(nullable: false),
                        TrackName = c.String(),
                        ConfigName = c.String(),
                        TrackCategoryId = c.Int(nullable: false),
                        Category = c.String(),
                        WeatherType = c.Int(nullable: false),
                        TempUnits = c.Int(nullable: false),
                        TempValue = c.Int(nullable: false),
                        RelHumidity = c.Int(nullable: false),
                        Fog = c.Int(nullable: false),
                        WindDir = c.Int(nullable: false),
                        WindUnits = c.Int(nullable: false),
                        Skies = c.Int(nullable: false),
                        WeatherVarInitial = c.Int(nullable: false),
                        WeatherVarOngoing = c.Int(nullable: false),
                        SimStartUTCTime = c.DateTime(),
                        SimStartUTCOffset = c.Long(nullable: false),
                        LeaveMarbles = c.Boolean(nullable: false),
                        PracticeRubber = c.Int(nullable: false),
                        QualifyRubber = c.Int(nullable: false),
                        WarmupRubber = c.Int(nullable: false),
                        RaceRubber = c.Int(nullable: false),
                        PracticeGripCompound = c.Int(nullable: false),
                        QualifyGripCompund = c.Int(nullable: false),
                        WarmupGripCompound = c.Int(nullable: false),
                        RaceGripCompound = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResultId)
                .ForeignKey("dbo.ResultEntities", t => t.ResultId)
                .Index(t => t.ResultId);
            
            CreateTable(
                "dbo.ResultsFilterOptionEntities",
                c => new
                    {
                        ResultsFilterId = c.Long(nullable: false, identity: true),
                        ScoringId = c.Long(nullable: false),
                        ResultsFilterType = c.String(),
                        ColumnPropertyName = c.String(),
                        Comparator = c.Int(nullable: false),
                        Exclude = c.Boolean(nullable: false),
                        FilterValues = c.String(),
                    })
                .PrimaryKey(t => t.ResultsFilterId)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringId, cascadeDelete: true)
                .Index(t => t.ScoringId);
            
            AddColumn("dbo.ResultRowEntities", "SimSessionType", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "OldIRating", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "NewIRating", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "SeasonStartIRating", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "License", c => c.String());
            AddColumn("dbo.ResultRowEntities", "SafetyRating", c => c.Double(nullable: false));
            AddColumn("dbo.ResultRowEntities", "OldCpi", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "NewCpi", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "ClubId", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "ClubName", c => c.String());
            AddColumn("dbo.ResultRowEntities", "CarId", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "CompletedPct", c => c.Double(nullable: false));
            AddColumn("dbo.ResultRowEntities", "QualifyingTimeAt", c => c.DateTime());
            AddColumn("dbo.ResultRowEntities", "Division", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "OldLicenseLevel", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "NewLicenseLevel", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "NumPitStops", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "PittedLaps", c => c.String());
            AddColumn("dbo.ResultRowEntities", "NumOfftrackLaps", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "OfftrackLaps", c => c.String());
            AddColumn("dbo.ResultRowEntities", "NumContactLaps", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "ContactLaps", c => c.String());
            AddColumn("dbo.ResultEntities", "SeasonId", c => c.Long());
            AddColumn("dbo.ReviewPenaltyEntities", "ReviewVoteId", c => c.Long());
            CreateIndex("dbo.ResultEntities", "SeasonId");
            CreateIndex("dbo.ReviewPenaltyEntities", "ReviewVoteId");
            AddForeignKey("dbo.ResultEntities", "SeasonId", "dbo.SeasonEntities", "SeasonId");
            AddForeignKey("dbo.ReviewPenaltyEntities", "ReviewVoteId", "dbo.AcceptedReviewVoteEntities", "ReviewVoteId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReviewPenaltyEntities", "ReviewVoteId", "dbo.AcceptedReviewVoteEntities");
            DropForeignKey("dbo.ResultEntities", "SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.ResultsFilterOptionEntities", "ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.IRSimSessionDetailsEntities", "ResultId", "dbo.ResultEntities");
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "ReviewVoteId" });
            DropIndex("dbo.ResultsFilterOptionEntities", new[] { "ScoringId" });
            DropIndex("dbo.IRSimSessionDetailsEntities", new[] { "ResultId" });
            DropIndex("dbo.ResultEntities", new[] { "SeasonId" });
            DropColumn("dbo.ReviewPenaltyEntities", "ReviewVoteId");
            DropColumn("dbo.ResultEntities", "SeasonId");
            DropColumn("dbo.ResultRowEntities", "ContactLaps");
            DropColumn("dbo.ResultRowEntities", "NumContactLaps");
            DropColumn("dbo.ResultRowEntities", "OfftrackLaps");
            DropColumn("dbo.ResultRowEntities", "NumOfftrackLaps");
            DropColumn("dbo.ResultRowEntities", "PittedLaps");
            DropColumn("dbo.ResultRowEntities", "NumPitStops");
            DropColumn("dbo.ResultRowEntities", "NewLicenseLevel");
            DropColumn("dbo.ResultRowEntities", "OldLicenseLevel");
            DropColumn("dbo.ResultRowEntities", "Division");
            DropColumn("dbo.ResultRowEntities", "QualifyingTimeAt");
            DropColumn("dbo.ResultRowEntities", "CompletedPct");
            DropColumn("dbo.ResultRowEntities", "CarId");
            DropColumn("dbo.ResultRowEntities", "ClubName");
            DropColumn("dbo.ResultRowEntities", "ClubId");
            DropColumn("dbo.ResultRowEntities", "NewCpi");
            DropColumn("dbo.ResultRowEntities", "OldCpi");
            DropColumn("dbo.ResultRowEntities", "SafetyRating");
            DropColumn("dbo.ResultRowEntities", "License");
            DropColumn("dbo.ResultRowEntities", "SeasonStartIRating");
            DropColumn("dbo.ResultRowEntities", "NewIRating");
            DropColumn("dbo.ResultRowEntities", "OldIRating");
            DropColumn("dbo.ResultRowEntities", "SimSessionType");
            DropTable("dbo.ResultsFilterOptionEntities");
            DropTable("dbo.IRSimSessionDetailsEntities");
        }
    }
}
