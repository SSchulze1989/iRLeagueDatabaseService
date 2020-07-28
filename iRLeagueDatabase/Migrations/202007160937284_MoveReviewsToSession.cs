namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveReviewsToSession : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IncidentReviewEntities", "Result_ResultId", "dbo.ResultEntities");
            DropIndex("dbo.IncidentReviewEntities", new[] { "Result_ResultId" });
            AddColumn("dbo.IncidentReviewEntities", "Session_SessionId", c => c.Long(nullable: false));
            CreateIndex("dbo.IncidentReviewEntities", "Session_SessionId");
            AddForeignKey("dbo.IncidentReviewEntities", "Session_SessionId", "dbo.SessionBaseEntities", "SessionId", cascadeDelete: true);
            DropColumn("dbo.IncidentReviewEntities", "Result_ResultId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IncidentReviewEntities", "Result_ResultId", c => c.Long(nullable: false));
            DropForeignKey("dbo.IncidentReviewEntities", "Session_SessionId", "dbo.SessionBaseEntities");
            DropIndex("dbo.IncidentReviewEntities", new[] { "Session_SessionId" });
            DropColumn("dbo.IncidentReviewEntities", "Session_SessionId");
            CreateIndex("dbo.IncidentReviewEntities", "Result_ResultId");
            AddForeignKey("dbo.IncidentReviewEntities", "Result_ResultId", "dbo.ResultEntities", "ResultId", cascadeDelete: true);
        }
    }
}
