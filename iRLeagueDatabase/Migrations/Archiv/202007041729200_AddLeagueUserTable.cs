namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeagueUserTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeagueUserEntities",
                c => new
                    {
                        AdminId = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        PwSalt = c.Binary(),
                        PwHash = c.Binary(),
                        MemberId = c.Long(),
                        AdminRights = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdminId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberId)
                .Index(t => t.MemberId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LeagueUserEntities", "MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.LeagueUserEntities", new[] { "MemberId" });
            DropTable("dbo.LeagueUserEntities");
        }
    }
}
