namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ScoredResultsRow_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScoredResultRowEntities",
                c => new
                    {
                        ResultRowId = c.Long(nullable: false),
                        ResultId = c.Long(nullable: false),
                        ScoringId = c.Long(nullable: false),
                        RacePoints = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        PenaltyPoints = c.Int(nullable: false),
                        FinalPosition = c.Int(nullable: false),
                        FinalPositionChange = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRowId, t.ResultId, t.ScoringId })
                .ForeignKey("dbo.ResultRowEntities", t => new { t.ResultRowId, t.ResultId }, cascadeDelete: true)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringId, cascadeDelete: true)
                .Index(t => new { t.ResultRowId, t.ResultId })
                .Index(t => t.ScoringId);
            
            DropColumn("dbo.ResultRowEntities", "FinalPosition");
            DropColumn("dbo.ResultRowEntities", "RacePoints");
            DropColumn("dbo.ResultRowEntities", "BonusPoints");
            DropColumn("dbo.ResultRowEntities", "PenaltyPoints");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResultRowEntities", "PenaltyPoints", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "BonusPoints", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "RacePoints", c => c.Int(nullable: false));
            AddColumn("dbo.ResultRowEntities", "FinalPosition", c => c.Int(nullable: false));
            DropForeignKey("dbo.ScoredResultRowEntities", "ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" }, "dbo.ResultRowEntities");
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoringId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" });
            DropTable("dbo.ScoredResultRowEntities");
        }
    }
}
