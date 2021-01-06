namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFirstLastRaceDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DriverStatisticRowEntities", "FirstSessionDate", c => c.DateTime());
            AddColumn("dbo.DriverStatisticRowEntities", "FirstRaceDate", c => c.DateTime());
            AddColumn("dbo.DriverStatisticRowEntities", "LastSessionDate", c => c.DateTime());
            AddColumn("dbo.DriverStatisticRowEntities", "LastRaceDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DriverStatisticRowEntities", "LastRaceDate");
            DropColumn("dbo.DriverStatisticRowEntities", "LastSessionDate");
            DropColumn("dbo.DriverStatisticRowEntities", "FirstRaceDate");
            DropColumn("dbo.DriverStatisticRowEntities", "FirstSessionDate");
        }
    }
}
