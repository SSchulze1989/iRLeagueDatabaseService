namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsMultiScoringProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "IsMultiScoring", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoringEntities", "IsMultiScoring");
        }
    }
}
