namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Restor_CompositeKey_ScoredResult : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoredResultRowEntities", "ScoredResultId", "dbo.ScoredResultEntities");
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId" });
            DropPrimaryKey("dbo.ScoredResultEntities");
            AddColumn("dbo.ScoredResultRowEntities", "ScoringId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.ScoredResultEntities", new[] { "ResultId", "ScoringId" });
            CreateIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            AddForeignKey("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities", new[] { "ResultId", "ScoringId" }, cascadeDelete: false);
            DropColumn("dbo.ScoredResultEntities", "ScoredResultId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoredResultEntities", "ScoredResultId", c => c.Long(nullable: false, identity: true));
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities");
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            DropPrimaryKey("dbo.ScoredResultEntities");
            DropColumn("dbo.ScoredResultRowEntities", "ScoringId");
            AddPrimaryKey("dbo.ScoredResultEntities", "ScoredResultId");
            CreateIndex("dbo.ScoredResultRowEntities", "ScoredResultId");
            AddForeignKey("dbo.ScoredResultRowEntities", "ScoredResultId", "dbo.ScoredResultEntities", "ScoredResultId", cascadeDelete: false);
        }
    }
}
