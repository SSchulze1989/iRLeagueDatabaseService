namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeScoredResultType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoredResultEntities", "ResultId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", "ScoringId", "dbo.ScoringEntities");
            AddColumn("dbo.ScoredResultEntities", "FastestLap", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultEntities", "FastestQualyLap", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultEntities", "FastestAvgLap", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultEntities", "FastestAvgLapDriver_MemberId", c => c.Long());
            AddColumn("dbo.ScoredResultEntities", "FastestQualyLapDriver_MemberId", c => c.Long());
            CreateIndex("dbo.ScoredResultEntities", "FastestAvgLapDriver_MemberId");
            CreateIndex("dbo.ScoredResultEntities", "FastestQualyLapDriver_MemberId");
            AddForeignKey("dbo.ScoredResultEntities", "FastestAvgLapDriver_MemberId", "dbo.LeagueMemberEntities", "MemberId");
            AddForeignKey("dbo.ScoredResultEntities", "FastestQualyLapDriver_MemberId", "dbo.LeagueMemberEntities", "MemberId");
            AddForeignKey("dbo.ScoredResultEntities", "ResultId", "dbo.ResultEntities", "ResultId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoredResultEntities", "ResultId", "dbo.ResultEntities");
            DropForeignKey("dbo.ScoredResultEntities", "FastestQualyLapDriver_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResultEntities", "FastestAvgLapDriver_MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.ScoredResultEntities", new[] { "FastestQualyLapDriver_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "FastestAvgLapDriver_MemberId" });
            DropColumn("dbo.ScoredResultEntities", "FastestQualyLapDriver_MemberId");
            DropColumn("dbo.ScoredResultEntities", "FastestAvgLapDriver_MemberId");
            DropColumn("dbo.ScoredResultEntities", "FastestAvgLap");
            DropColumn("dbo.ScoredResultEntities", "FastestQualyLap");
            DropColumn("dbo.ScoredResultEntities", "FastestLap");
            AddForeignKey("dbo.ScoredResultRowEntities", "ScoringId", "dbo.ScoringEntities", "ScoringId", cascadeDelete: true);
            AddForeignKey("dbo.ScoredResultEntities", "ResultId", "dbo.SessionBaseEntities", "SessionId", cascadeDelete: true);
        }
    }
}
