namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLapTimesToTemResultsRow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoredTeamResultRowEntities", "AvgLapTime", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredTeamResultRowEntities", "FastestLapTime", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoredTeamResultRowEntities", "FastestLapTime");
            DropColumn("dbo.ScoredTeamResultRowEntities", "AvgLapTime");
        }
    }
}
