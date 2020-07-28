namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewPenalty : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReviewPenaltyEntities",
                c => new
                    {
                        ResultRowId = c.Long(nullable: false),
                        ReviewId = c.Long(nullable: false),
                        PenaltyPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRowId, t.ReviewId })
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewId, cascadeDelete: true)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.ResultRowId, cascadeDelete: false)
                .Index(t => t.ResultRowId)
                .Index(t => t.ReviewId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReviewPenaltyEntities", "ResultRowId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.ReviewPenaltyEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "ReviewId" });
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "ResultRowId" });
            DropTable("dbo.ReviewPenaltyEntities");
        }
    }
}
