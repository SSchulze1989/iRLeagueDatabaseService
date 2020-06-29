namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultEntity_Revision : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoredResultEntities", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.ScoredResultEntities", "LastModifiedOn", c => c.DateTime());
            AddColumn("dbo.ScoredResultEntities", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.ScoredResultEntities", "CreatedBy_MemberId", c => c.Long());
            AddColumn("dbo.ScoredResultEntities", "LastModifiedBy_MemberId", c => c.Long());
            CreateIndex("dbo.ScoredResultEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.ScoredResultEntities", "LastModifiedBy_MemberId");
            AddForeignKey("dbo.ScoredResultEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities", "MemberId");
            AddForeignKey("dbo.ScoredResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities", "MemberId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoredResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResultEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.ScoredResultEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "CreatedBy_MemberId" });
            DropColumn("dbo.ScoredResultEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.ScoredResultEntities", "CreatedBy_MemberId");
            DropColumn("dbo.ScoredResultEntities", "Version");
            DropColumn("dbo.ScoredResultEntities", "LastModifiedOn");
            DropColumn("dbo.ScoredResultEntities", "CreatedOn");
        }
    }
}
