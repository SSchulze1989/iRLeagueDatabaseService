namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLeagueUserTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LeagueUserEntities", "MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.LeagueUserEntities", new[] { "MemberId" });
            DropTable("dbo.LeagueUserEntities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.LeagueUserEntities",
                c => new
                    {
                        AdminId = c.Long(nullable: false, identity: true),
                        UserIdentityId = c.String(),
                        PwSalt = c.Binary(),
                        PwHash = c.Binary(),
                        MemberId = c.Long(),
                        AdminRights = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdminId);
            
            CreateIndex("dbo.LeagueUserEntities", "MemberId");
            AddForeignKey("dbo.LeagueUserEntities", "MemberId", "dbo.LeagueMemberEntities", "MemberId");
        }
    }
}
