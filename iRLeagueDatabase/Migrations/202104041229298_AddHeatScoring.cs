namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHeatScoring : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SessionBaseEntities", new[] { "ScheduleId" });
            AddColumn("dbo.ScoredResultRowEntities", "PointsEligible", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScoringEntities", "ParentScoringId", c => c.Long());
            AddColumn("dbo.ScoringEntities", "ScoringSessionType", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "SessionSelectType", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "ScoringWeightValues", c => c.String());
            AddColumn("dbo.ScoringEntities", "AccumulateBy", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "AccumulateResultsOption", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "PointsSortOptions", c => c.String());
            AddColumn("dbo.ScoringEntities", "FinalSortOptions", c => c.String());
            AddColumn("dbo.SessionBaseEntities", "Name", c => c.String());
            AddColumn("dbo.SessionBaseEntities", "ParentSessionId", c => c.Long());
            AddColumn("dbo.SessionBaseEntities", "SubSessionNr", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "RacePoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "BonusPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "PenaltyPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "TotalPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "RacePoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "BonusPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "PenaltyPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "FinalPositionChange", c => c.Double(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "TotalPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "StartPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "FinishPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "CompletedLaps", c => c.Double(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "LeadLaps", c => c.Double(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "Incidents", c => c.Double(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "PositionChange", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "RacePoints", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "TotalPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "BonusPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "Incidents", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "PenaltyPoints", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "LeadingLaps", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "CompletedLaps", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "BestFinishPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "WorstFinishPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "FirstRaceFinishPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "LastRaceFinishPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "BestStartPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "WorstStartPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "FirstRaceStartPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "LastRaceStartPosition", c => c.Double(nullable: false));
            AlterColumn("dbo.SessionBaseEntities", "ScheduleId", c => c.Long());
            CreateIndex("dbo.ScoringEntities", "ParentScoringId");
            CreateIndex("dbo.SessionBaseEntities", "ScheduleId");
            CreateIndex("dbo.SessionBaseEntities", "ParentSessionId");
            AddForeignKey("dbo.SessionBaseEntities", "ParentSessionId", "dbo.SessionBaseEntities", "SessionId");
            AddForeignKey("dbo.ScoringEntities", "ParentScoringId", "dbo.ScoringEntities", "ScoringId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoringEntities", "ParentScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.SessionBaseEntities", "ParentSessionId", "dbo.SessionBaseEntities");
            DropIndex("dbo.SessionBaseEntities", new[] { "ParentSessionId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "ScheduleId" });
            DropIndex("dbo.ScoringEntities", new[] { "ParentScoringId" });
            AlterColumn("dbo.SessionBaseEntities", "ScheduleId", c => c.Long(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "LastRaceStartPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "FirstRaceStartPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "WorstStartPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "BestStartPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "LastRaceFinishPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "FirstRaceFinishPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "WorstFinishPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "BestFinishPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "CompletedLaps", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "LeadingLaps", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "PenaltyPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "Incidents", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "BonusPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "TotalPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.DriverStatisticRowEntities", "RacePoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "PositionChange", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "Incidents", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "LeadLaps", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "CompletedLaps", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "FinishPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.ResultRowEntities", "StartPosition", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "TotalPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "FinalPositionChange", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "PenaltyPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "BonusPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredResultRowEntities", "RacePoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "TotalPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "PenaltyPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "BonusPoints", c => c.Int(nullable: false));
            AlterColumn("dbo.ScoredTeamResultRowEntities", "RacePoints", c => c.Int(nullable: false));
            DropColumn("dbo.SessionBaseEntities", "SubSessionNr");
            DropColumn("dbo.SessionBaseEntities", "ParentSessionId");
            DropColumn("dbo.SessionBaseEntities", "Name");
            DropColumn("dbo.ScoringEntities", "FinalSortOptions");
            DropColumn("dbo.ScoringEntities", "PointsSortOptions");
            DropColumn("dbo.ScoringEntities", "AccumulateResultsOption");
            DropColumn("dbo.ScoringEntities", "AccumulateBy");
            DropColumn("dbo.ScoringEntities", "ScoringWeightValues");
            DropColumn("dbo.ScoringEntities", "SessionSelectType");
            DropColumn("dbo.ScoringEntities", "ScoringSessionType");
            DropColumn("dbo.ScoringEntities", "ParentScoringId");
            DropColumn("dbo.ScoredResultRowEntities", "PointsEligible");
            CreateIndex("dbo.SessionBaseEntities", "ScheduleId");
        }
    }
}
