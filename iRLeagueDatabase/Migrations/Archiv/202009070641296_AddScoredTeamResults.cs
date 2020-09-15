namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScoredTeamResults : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScoredTeamResultRowEntities",
                c => new
                    {
                        ScoredResultRowId = c.Long(nullable: false, identity: true),
                        ScoredResultId = c.Long(nullable: false),
                        ScoringId = c.Long(nullable: false),
                        ResultRowId = c.Long(nullable: false),
                        TeamId = c.Long(nullable: false),
                        RacePoints = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        PenaltyPoints = c.Int(nullable: false),
                        FinalPosition = c.Int(nullable: false),
                        FinalPositionChange = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScoredResultRowId)
                .ForeignKey("dbo.ResultRowEntities", t => t.ResultRowId, cascadeDelete: false)
                .ForeignKey("dbo.TeamEntities", t => t.TeamId, cascadeDelete: false)
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ScoredResultId, t.ScoringId }, cascadeDelete: false)
                .Index(t => new { t.ScoredResultId, t.ScoringId })
                .Index(t => t.ResultRowId)
                .Index(t => t.TeamId);
            
            AddColumn("dbo.ScoredResultRowEntities", "TeamResultRowId", c => c.Long());
            AddColumn("dbo.ScoredResultEntities", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.ScoredResultRowEntities", "TeamResultRowId");
            AddForeignKey("dbo.ScoredResultRowEntities", "TeamResultRowId", "dbo.ScoredTeamResultRowEntities", "ScoredResultRowId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoredTeamResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredTeamResultRowEntities", "TeamId", "dbo.TeamEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", "TeamResultRowId", "dbo.ScoredTeamResultRowEntities");
            DropForeignKey("dbo.ScoredTeamResultRowEntities", "ResultRowId", "dbo.ResultRowEntities");
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "TeamId" });
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "ResultRowId" });
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "TeamResultRowId" });
            DropColumn("dbo.ScoredResultEntities", "Discriminator");
            DropColumn("dbo.ScoredResultRowEntities", "TeamResultRowId");
            DropTable("dbo.ScoredTeamResultRowEntities");
        }
    }
}
