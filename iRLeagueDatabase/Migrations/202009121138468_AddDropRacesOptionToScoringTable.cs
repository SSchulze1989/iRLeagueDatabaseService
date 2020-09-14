namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDropRacesOptionToScoringTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringTableEntities", "DropRacesOption", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringTableEntities", "ResultsPerRaceCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoringTableEntities", "ResultsPerRaceCount");
            DropColumn("dbo.ScoringTableEntities", "DropRacesOption");
        }
    }
}
