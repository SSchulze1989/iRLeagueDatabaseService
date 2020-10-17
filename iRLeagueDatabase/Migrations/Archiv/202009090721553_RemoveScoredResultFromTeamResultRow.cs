namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveScoredResultFromTeamResultRow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoredTeamResultRowEntities", "ResultRowId", "dbo.ResultRowEntities");
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "ResultRowId" });
            DropColumn("dbo.ScoredTeamResultRowEntities", "ResultRowId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoredTeamResultRowEntities", "ResultRowId", c => c.Long(nullable: false));
            CreateIndex("dbo.ScoredTeamResultRowEntities", "ResultRowId");
            AddForeignKey("dbo.ScoredTeamResultRowEntities", "ResultRowId", "dbo.ResultRowEntities", "ResultRowId", cascadeDelete: true);
        }
    }
}
