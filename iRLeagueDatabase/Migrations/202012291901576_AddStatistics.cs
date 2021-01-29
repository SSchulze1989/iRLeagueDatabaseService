namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatistics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StatisticSetEntities",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        UpdateInterval = c.Long(nullable: false),
                        UpdateTime = c.DateTime(),
                        RequiresRecalculation = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedByUserId = c.String(),
                        CreatedByUserName = c.String(),
                        LastModifiedByUserId = c.String(),
                        LastModifiedByUserName = c.String(),
                        CurrentChampId = c.Long(),
                        SeasonId = c.Long(),
                        ScoringTableId = c.Long(),
                        FinishedRaces = c.Int(),
                        IsSeasonFinished = c.Boolean(),
                        ImportSource = c.String(),
                        Description = c.String(),
                        FirstDate = c.DateTime(),
                        LastDate = c.DateTime(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ScoringTableEntities", t => t.ScoringTableId)
                .ForeignKey("dbo.SeasonEntities", t => t.SeasonId, cascadeDelete: true)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CurrentChampId)
                .Index(t => t.CurrentChampId)
                .Index(t => t.SeasonId)
                .Index(t => t.ScoringTableId);
            
            CreateTable(
                "dbo.DriverStatisticRowEntities",
                c => new
                    {
                        StatisticSetId = c.Long(nullable: false),
                        MemberId = c.Long(nullable: false),
                        FirstResultRowId = c.Long(),
                        LastResultRowId = c.Long(),
                        StartIRating = c.Int(nullable: false),
                        EndIRating = c.Int(nullable: false),
                        StartSRating = c.Double(nullable: false),
                        EndSRating = c.Double(nullable: false),
                        FirstSessionId = c.Long(),
                        FirstSessionDate = c.DateTime(),
                        FirstRaceId = c.Long(),
                        FirstRaceDate = c.DateTime(),
                        LastSessionId = c.Long(),
                        LastSessionDate = c.DateTime(),
                        LastRaceId = c.Long(),
                        LastRaceDate = c.DateTime(),
                        RacePoints = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        Races = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        Poles = c.Int(nullable: false),
                        Top3 = c.Int(nullable: false),
                        Top5 = c.Int(nullable: false),
                        Top10 = c.Int(nullable: false),
                        Top15 = c.Int(nullable: false),
                        Top20 = c.Int(nullable: false),
                        Top25 = c.Int(nullable: false),
                        RacesInPoints = c.Int(nullable: false),
                        RacesCompleted = c.Int(nullable: false),
                        Incidents = c.Int(nullable: false),
                        PenaltyPoints = c.Int(nullable: false),
                        FastestLaps = c.Int(nullable: false),
                        IncidentsUnderInvestigation = c.Int(nullable: false),
                        IncidentsWithPenalty = c.Int(nullable: false),
                        LeadingLaps = c.Int(nullable: false),
                        CompletedLaps = c.Int(nullable: false),
                        CurrentSeasonPosition = c.Int(nullable: false),
                        DrivenKm = c.Double(nullable: false),
                        LeadingKm = c.Double(nullable: false),
                        AvgFinishPosition = c.Double(nullable: false),
                        AvgFinalPosition = c.Double(nullable: false),
                        AvgStartPosition = c.Double(nullable: false),
                        AvgPointsPerRace = c.Double(nullable: false),
                        AvgIncidentsPerRace = c.Double(nullable: false),
                        AvgIncidentsPerLap = c.Double(nullable: false),
                        AvgIncidentsPerKm = c.Double(nullable: false),
                        AvgPenaltyPointsPerRace = c.Double(nullable: false),
                        AvgPenaltyPointsPerLap = c.Double(nullable: false),
                        AvgPenaltyPointsPerKm = c.Double(nullable: false),
                        AvgIRating = c.Double(nullable: false),
                        AvgSRating = c.Double(nullable: false),
                        BestFinishPosition = c.Int(nullable: false),
                        WorstFinishPosition = c.Int(nullable: false),
                        FirstRaceFinishPosition = c.Int(nullable: false),
                        LastRaceFinishPosition = c.Int(nullable: false),
                        BestFinalPosition = c.Int(nullable: false),
                        WorstFinalPosition = c.Int(nullable: false),
                        FirstRaceFinalPosition = c.Int(nullable: false),
                        LastRaceFinalPosition = c.Int(nullable: false),
                        BestStartPosition = c.Int(nullable: false),
                        WorstStartPosition = c.Int(nullable: false),
                        FirstRaceStartPosition = c.Int(nullable: false),
                        LastRaceStartPosition = c.Int(nullable: false),
                        Titles = c.Int(nullable: false),
                        HardChargerAwards = c.Int(nullable: false),
                        CleanestDriverAwards = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StatisticSetId, t.MemberId })
                .ForeignKey("dbo.SessionBaseEntities", t => t.FirstRaceId)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.FirstResultRowId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.FirstSessionId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.LastRaceId)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.LastResultRowId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.LastSessionId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberId, cascadeDelete: true)
                .ForeignKey("dbo.StatisticSetEntities", t => t.StatisticSetId, cascadeDelete: true)
                .Index(t => t.StatisticSetId)
                .Index(t => t.MemberId)
                .Index(t => t.FirstResultRowId)
                .Index(t => t.LastResultRowId)
                .Index(t => t.FirstSessionId)
                .Index(t => t.FirstRaceId)
                .Index(t => t.LastSessionId)
                .Index(t => t.LastRaceId);
            
            CreateTable(
                "dbo.ScoredResult_CleanestDrivers",
                c => new
                    {
                        ResultRefId = c.Long(nullable: false),
                        ScoringRefId = c.Long(nullable: false),
                        LeagueMemberRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRefId, t.ScoringRefId, t.LeagueMemberRefId })
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ResultRefId, t.ScoringRefId }, cascadeDelete: false)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LeagueMemberRefId, cascadeDelete: true)
                .Index(t => new { t.ResultRefId, t.ScoringRefId })
                .Index(t => t.LeagueMemberRefId);
            
            CreateTable(
                "dbo.ScoredResult_HardChargers",
                c => new
                    {
                        ResultRefId = c.Long(nullable: false),
                        ScoringRefId = c.Long(nullable: false),
                        LeagueMemberRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRefId, t.ScoringRefId, t.LeagueMemberRefId })
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ResultRefId, t.ScoringRefId }, cascadeDelete: false)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LeagueMemberRefId, cascadeDelete: true)
                .Index(t => new { t.ResultRefId, t.ScoringRefId })
                .Index(t => t.LeagueMemberRefId);
            
            CreateTable(
                "dbo.LeagueStatisticSet_SeasonStatisticSet",
                c => new
                    {
                        LeagueStatisticSetRefId = c.Long(nullable: false),
                        SeasonStatisticSetRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.LeagueStatisticSetRefId, t.SeasonStatisticSetRefId })
                .ForeignKey("dbo.StatisticSetEntities", t => t.LeagueStatisticSetRefId)
                .ForeignKey("dbo.StatisticSetEntities", t => t.SeasonStatisticSetRefId)
                .Index(t => t.LeagueStatisticSetRefId)
                .Index(t => t.SeasonStatisticSetRefId);
            
            AddColumn("dbo.ResultEntities", "PoleLaptime", c => c.Long(nullable: false));
            AddColumn("dbo.SeasonEntities", "Finished", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResultsFilterOptionEntities", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.ResultsFilterOptionEntities", "LastModifiedOn", c => c.DateTime());
            AddColumn("dbo.ResultsFilterOptionEntities", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.ResultsFilterOptionEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.ResultsFilterOptionEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.ResultsFilterOptionEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.ResultsFilterOptionEntities", "LastModifiedByUserName", c => c.String());
            AlterColumn("dbo.IRSimSessionDetailsEntities", "IRSeasonName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LeagueStatisticSet_SeasonStatisticSet", "SeasonStatisticSetRefId", "dbo.StatisticSetEntities");
            DropForeignKey("dbo.LeagueStatisticSet_SeasonStatisticSet", "LeagueStatisticSetRefId", "dbo.StatisticSetEntities");
            DropForeignKey("dbo.StatisticSetEntities", "CurrentChampId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.StatisticSetEntities", "SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.StatisticSetEntities", "ScoringTableId", "dbo.ScoringTableEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "StatisticSetId", "dbo.StatisticSetEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "LastSessionId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "LastResultRowId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "LastRaceId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "FirstSessionId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "FirstResultRowId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "FirstRaceId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.ScoredResult_HardChargers", "LeagueMemberRefId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResult_HardChargers", new[] { "ResultRefId", "ScoringRefId" }, "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredResult_CleanestDrivers", "LeagueMemberRefId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResult_CleanestDrivers", new[] { "ResultRefId", "ScoringRefId" }, "dbo.ScoredResultEntities");
            DropIndex("dbo.LeagueStatisticSet_SeasonStatisticSet", new[] { "SeasonStatisticSetRefId" });
            DropIndex("dbo.LeagueStatisticSet_SeasonStatisticSet", new[] { "LeagueStatisticSetRefId" });
            DropIndex("dbo.ScoredResult_HardChargers", new[] { "LeagueMemberRefId" });
            DropIndex("dbo.ScoredResult_HardChargers", new[] { "ResultRefId", "ScoringRefId" });
            DropIndex("dbo.ScoredResult_CleanestDrivers", new[] { "LeagueMemberRefId" });
            DropIndex("dbo.ScoredResult_CleanestDrivers", new[] { "ResultRefId", "ScoringRefId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "LastRaceId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "LastSessionId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "FirstRaceId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "FirstSessionId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "LastResultRowId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "FirstResultRowId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "MemberId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "StatisticSetId" });
            DropIndex("dbo.StatisticSetEntities", new[] { "ScoringTableId" });
            DropIndex("dbo.StatisticSetEntities", new[] { "SeasonId" });
            DropIndex("dbo.StatisticSetEntities", new[] { "CurrentChampId" });
            AlterColumn("dbo.IRSimSessionDetailsEntities", "IRSeasonName", c => c.Long(nullable: false));
            DropColumn("dbo.ResultsFilterOptionEntities", "LastModifiedByUserName");
            DropColumn("dbo.ResultsFilterOptionEntities", "LastModifiedByUserId");
            DropColumn("dbo.ResultsFilterOptionEntities", "CreatedByUserName");
            DropColumn("dbo.ResultsFilterOptionEntities", "CreatedByUserId");
            DropColumn("dbo.ResultsFilterOptionEntities", "Version");
            DropColumn("dbo.ResultsFilterOptionEntities", "LastModifiedOn");
            DropColumn("dbo.ResultsFilterOptionEntities", "CreatedOn");
            DropColumn("dbo.SeasonEntities", "Finished");
            DropColumn("dbo.ResultEntities", "PoleLaptime");
            DropTable("dbo.LeagueStatisticSet_SeasonStatisticSet");
            DropTable("dbo.ScoredResult_HardChargers");
            DropTable("dbo.ScoredResult_CleanestDrivers");
            DropTable("dbo.DriverStatisticRowEntities");
            DropTable("dbo.StatisticSetEntities");
        }
    }
}
