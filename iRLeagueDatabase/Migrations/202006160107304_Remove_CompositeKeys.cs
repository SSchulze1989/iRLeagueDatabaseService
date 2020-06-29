namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_CompositeKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" }, "dbo.ResultRowEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities");
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            DropPrimaryKey("dbo.ResultRowEntities");
            DropPrimaryKey("dbo.ScoredResultRowEntities");
            DropPrimaryKey("dbo.ScoredResultEntities");
            AddColumn("dbo.ScoredResultRowEntities", "ScoredResultRowId", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.ScoredResultEntities", "ScoredResultId", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.ResultRowEntities", "ResultRowId");
            AddPrimaryKey("dbo.ScoredResultRowEntities", "ScoredResultRowId");
            AddPrimaryKey("dbo.ScoredResultEntities", "ScoredResultId");
            CreateIndex("dbo.ScoredResultRowEntities", "ResultRowId");
            CreateIndex("dbo.ScoredResultRowEntities", "ScoredResultId");
            AddForeignKey("dbo.ScoredResultRowEntities", "ResultRowId", "dbo.ResultRowEntities", "ResultRowId", cascadeDelete: true);
            AddForeignKey("dbo.ScoredResultRowEntities", "ScoredResultId", "dbo.ScoredResultEntities", "ScoredResultId", cascadeDelete: false);
            DropColumn("dbo.ScoredResultRowEntities", "ResultId");
            DropColumn("dbo.ScoredResultRowEntities", "ScoringId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoredResultRowEntities", "ScoringId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultRowEntities", "ResultId", c => c.Long(nullable: false));
            DropForeignKey("dbo.ScoredResultRowEntities", "ScoredResultId", "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", "ResultRowId", "dbo.ResultRowEntities");
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ResultRowId" });
            DropPrimaryKey("dbo.ScoredResultEntities");
            DropPrimaryKey("dbo.ScoredResultRowEntities");
            DropPrimaryKey("dbo.ResultRowEntities");
            DropColumn("dbo.ScoredResultEntities", "ScoredResultId");
            DropColumn("dbo.ScoredResultRowEntities", "ScoredResultRowId");
            AddPrimaryKey("dbo.ScoredResultEntities", new[] { "ResultId", "ScoringId" });
            AddPrimaryKey("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId", "ScoredResultId", "ScoringId" });
            AddPrimaryKey("dbo.ResultRowEntities", new[] { "ResultRowId", "ResultId" });
            CreateIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            CreateIndex("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" });
            AddForeignKey("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities", new[] { "ResultId", "ScoringId" }, cascadeDelete: true);
            AddForeignKey("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" }, "dbo.ResultRowEntities", new[] { "ResultRowId", "ResultId" }, cascadeDelete: true);
        }
    }
}
