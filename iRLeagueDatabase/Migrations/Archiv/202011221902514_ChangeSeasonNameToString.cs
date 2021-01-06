namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSeasonNameToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IRSimSessionDetailsEntities", "IRSeasonName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IRSimSessionDetailsEntities", "IRSeasonName", c => c.Long(nullable: false));
        }
    }
}
