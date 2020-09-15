namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKey_MemberId_ResultRow : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResultRowEntities", "Member_MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.ResultRowEntities", new[] { "Member_MemberId" });
            RenameColumn(table: "dbo.ResultRowEntities", name: "Member_MemberId", newName: "MemberId");
            AlterColumn("dbo.ResultRowEntities", "MemberId", c => c.Long(nullable: false));
            CreateIndex("dbo.ResultRowEntities", "MemberId");
            AddForeignKey("dbo.ResultRowEntities", "MemberId", "dbo.LeagueMemberEntities", "MemberId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultRowEntities", "MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.ResultRowEntities", new[] { "MemberId" });
            AlterColumn("dbo.ResultRowEntities", "MemberId", c => c.Long());
            RenameColumn(table: "dbo.ResultRowEntities", name: "MemberId", newName: "Member_MemberId");
            CreateIndex("dbo.ResultRowEntities", "Member_MemberId");
            AddForeignKey("dbo.ResultRowEntities", "Member_MemberId", "dbo.LeagueMemberEntities", "MemberId");
        }
    }
}
