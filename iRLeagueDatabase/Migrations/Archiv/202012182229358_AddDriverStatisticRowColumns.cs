namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDriverStatisticRowColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DriverStatisticRowEntities", "Titles", c => c.Int(nullable: false));
            AddColumn("dbo.DriverStatisticRowEntities", "HardChargerAwards", c => c.Int(nullable: false));
            AddColumn("dbo.DriverStatisticRowEntities", "CleanestDriverAwards", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DriverStatisticRowEntities", "CleanestDriverAwards");
            DropColumn("dbo.DriverStatisticRowEntities", "HardChargerAwards");
            DropColumn("dbo.DriverStatisticRowEntities", "Titles");
        }
    }
}
