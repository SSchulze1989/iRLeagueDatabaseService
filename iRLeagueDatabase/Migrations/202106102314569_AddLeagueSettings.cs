namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeagueSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeagueEntities", "IsPublic", c => c.Boolean(nullable: false));
            AddColumn("dbo.LeagueEntities", "IsActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LeagueEntities", "IsActive");
            DropColumn("dbo.LeagueEntities", "IsPublic");
        }
    }
}
