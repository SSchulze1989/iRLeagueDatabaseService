namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeamInfoToResults : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoredResultRowEntities", "TeamId", c => c.Long());
            AddColumn("dbo.ResultRowEntities", "TeamId", c => c.Long());
            AddColumn("dbo.ScoringEntities", "TakeTeamFromResulSet", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScoringEntities", "UpdateTeamOnRecalculation", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ScoredResultRowEntities", "TeamId");
            CreateIndex("dbo.ResultRowEntities", "TeamId");
            AddForeignKey("dbo.ResultRowEntities", "TeamId", "dbo.TeamEntities", "TeamId");
            AddForeignKey("dbo.ScoredResultRowEntities", "TeamId", "dbo.TeamEntities", "TeamId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoredResultRowEntities", "TeamId", "dbo.TeamEntities");
            DropForeignKey("dbo.ResultRowEntities", "TeamId", "dbo.TeamEntities");
            DropIndex("dbo.ResultRowEntities", new[] { "TeamId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "TeamId" });
            DropColumn("dbo.ScoringEntities", "UpdateTeamOnRecalculation");
            DropColumn("dbo.ScoringEntities", "TakeTeamFromResulSet");
            DropColumn("dbo.ResultRowEntities", "TeamId");
            DropColumn("dbo.ScoredResultRowEntities", "TeamId");
        }
    }
}
