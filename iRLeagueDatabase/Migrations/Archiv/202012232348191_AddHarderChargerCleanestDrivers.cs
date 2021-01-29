namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHarderChargerCleanestDrivers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScoredResult_CleanestDrivers",
                c => new
                    {
                        ResultRefId = c.Long(nullable: false),
                        ScoringRefId = c.Long(nullable: false),
                        LeagueMemberRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRefId, t.ScoringRefId, t.LeagueMemberRefId })
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ResultRefId, t.ScoringRefId }, cascadeDelete: false)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LeagueMemberRefId, cascadeDelete: true)
                .Index(t => new { t.ResultRefId, t.ScoringRefId })
                .Index(t => t.LeagueMemberRefId);
            
            CreateTable(
                "dbo.ScoredResult_HardChargers",
                c => new
                    {
                        ResultRefId = c.Long(nullable: false),
                        ScoringRefId = c.Long(nullable: false),
                        LeagueMemberRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRefId, t.ScoringRefId, t.LeagueMemberRefId })
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ResultRefId, t.ScoringRefId }, cascadeDelete: false)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LeagueMemberRefId, cascadeDelete: true)
                .Index(t => new { t.ResultRefId, t.ScoringRefId })
                .Index(t => t.LeagueMemberRefId);
            
            AddColumn("dbo.ResultEntities", "PoleLaptime", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoredResult_HardChargers", "LeagueMemberRefId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResult_HardChargers", new[] { "ResultRefId", "ScoringRefId" }, "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredResult_CleanestDrivers", "LeagueMemberRefId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResult_CleanestDrivers", new[] { "ResultRefId", "ScoringRefId" }, "dbo.ScoredResultEntities");
            DropIndex("dbo.ScoredResult_HardChargers", new[] { "LeagueMemberRefId" });
            DropIndex("dbo.ScoredResult_HardChargers", new[] { "ResultRefId", "ScoringRefId" });
            DropIndex("dbo.ScoredResult_CleanestDrivers", new[] { "LeagueMemberRefId" });
            DropIndex("dbo.ScoredResult_CleanestDrivers", new[] { "ResultRefId", "ScoringRefId" });
            DropColumn("dbo.ResultEntities", "PoleLaptime");
            DropTable("dbo.ScoredResult_HardChargers");
            DropTable("dbo.ScoredResult_CleanestDrivers");
        }
    }
}
