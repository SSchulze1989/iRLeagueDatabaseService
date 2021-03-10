namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSeasonStartEndDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeasonEntities", "SeasonStart", c => c.DateTime());
            AddColumn("dbo.SeasonEntities", "SeasonEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SeasonEntities", "SeasonEnd");
            DropColumn("dbo.SeasonEntities", "SeasonStart");
        }
    }
}
