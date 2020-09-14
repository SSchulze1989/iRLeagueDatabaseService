namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExternalScoringSource : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "ExtScoringSourceId", c => c.Long());
            AddColumn("dbo.ScoringEntities", "TakeResultsFromExtSource", c => c.Boolean(nullable: false));
            CreateIndex("dbo.ScoringEntities", "ExtScoringSourceId");
            AddForeignKey("dbo.ScoringEntities", "ExtScoringSourceId", "dbo.ScoringEntities", "ScoringId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoringEntities", "ExtScoringSourceId", "dbo.ScoringEntities");
            DropIndex("dbo.ScoringEntities", new[] { "ExtScoringSourceId" });
            DropColumn("dbo.ScoringEntities", "TakeResultsFromExtSource");
            DropColumn("dbo.ScoringEntities", "ExtScoringSourceId");
        }
    }
}
