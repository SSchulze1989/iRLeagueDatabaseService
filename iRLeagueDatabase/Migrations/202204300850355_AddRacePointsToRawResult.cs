namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRacePointsToRawResult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultRowEntities", "RacePoints", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResultRowEntities", "RacePoints");
        }
    }
}
