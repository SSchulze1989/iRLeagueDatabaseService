namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScoringKindProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "ScoringKind", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "MaxResultsPerGroup", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "TakeGroupAverage", c => c.Boolean(nullable: false));
            AddColumn("dbo.ScoringTableEntities", "ScoringKind", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoringTableEntities", "ScoringKind");
            DropColumn("dbo.ScoringEntities", "TakeGroupAverage");
            DropColumn("dbo.ScoringEntities", "MaxResultsPerGroup");
            DropColumn("dbo.ScoringEntities", "ScoringKind");
        }
    }
}
