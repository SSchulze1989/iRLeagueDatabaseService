namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShowResultsScoringOption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "ShowResults", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoringEntities", "ShowResults");
        }
    }
}
