namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserIdentityId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LeagueUserEntities", "UserIdentityId", c => c.String());
            DropColumn("dbo.LeagueUserEntities", "UserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LeagueUserEntities", "UserName", c => c.String());
            DropColumn("dbo.LeagueUserEntities", "UserIdentityId");
        }
    }
}
