namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateScoringModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoringEntities", "ScheduleId", "dbo.ScheduleEntities");
            DropIndex("dbo.ScoringEntities", new[] { "ScheduleId" });
            CreateTable(
                "dbo.MultiScoringMap",
                c => new
                    {
                        ScoringParentId = c.Int(nullable: false),
                        ScoringChildId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoringParentId, t.ScoringChildId })
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringParentId)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringChildId)
                .Index(t => t.ScoringParentId)
                .Index(t => t.ScoringChildId);
            
            CreateTable(
                "dbo.Scoring_Session",
                c => new
                    {
                        ScoringRefId = c.Int(nullable: false),
                        SessionRefId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoringRefId, t.SessionRefId })
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringRefId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.SessionRefId)
                .Index(t => t.ScoringRefId)
                .Index(t => t.SessionRefId);
            
            AddColumn("dbo.ScoringEntities", "Name", c => c.String());
            AddColumn("dbo.ScoringEntities", "SeasonId", c => c.Int(nullable: false));
            AddColumn("dbo.ScoringEntities", "BasePoints", c => c.String());
            AddColumn("dbo.ScoringEntities", "BonusPoints", c => c.String());
            AddColumn("dbo.ScoringEntities", "IncPenaltyPoints", c => c.String());
            AddColumn("dbo.ScoringEntities", "MultiScoringFactors", c => c.String());
            AddColumn("dbo.ScoringEntities", "SeasonEntity_SeasonId", c => c.Int());
            CreateIndex("dbo.ScoringEntities", "SeasonId");
            CreateIndex("dbo.ScoringEntities", "SeasonEntity_SeasonId");
            AddForeignKey("dbo.ScoringEntities", "SeasonId", "dbo.SeasonEntities", "SeasonId", cascadeDelete: true);
            AddForeignKey("dbo.ScoringEntities", "SeasonEntity_SeasonId", "dbo.SeasonEntities", "SeasonId");
            DropColumn("dbo.ScoringEntities", "ScheduleId");
            DropColumn("dbo.ScoringEntities", "ScoringRuleName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoringEntities", "ScoringRuleName", c => c.String());
            AddColumn("dbo.ScoringEntities", "ScheduleId", c => c.Int());
            DropForeignKey("dbo.ScoringEntities", "SeasonEntity_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.Scoring_Session", "SessionRefId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.Scoring_Session", "ScoringRefId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringEntities", "SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.MultiScoringMap", "ScoringChildId", "dbo.ScoringEntities");
            DropForeignKey("dbo.MultiScoringMap", "ScoringParentId", "dbo.ScoringEntities");
            DropIndex("dbo.Scoring_Session", new[] { "SessionRefId" });
            DropIndex("dbo.Scoring_Session", new[] { "ScoringRefId" });
            DropIndex("dbo.MultiScoringMap", new[] { "ScoringChildId" });
            DropIndex("dbo.MultiScoringMap", new[] { "ScoringParentId" });
            DropIndex("dbo.ScoringEntities", new[] { "SeasonEntity_SeasonId" });
            DropIndex("dbo.ScoringEntities", new[] { "SeasonId" });
            DropColumn("dbo.ScoringEntities", "SeasonEntity_SeasonId");
            DropColumn("dbo.ScoringEntities", "MultiScoringFactors");
            DropColumn("dbo.ScoringEntities", "IncPenaltyPoints");
            DropColumn("dbo.ScoringEntities", "BonusPoints");
            DropColumn("dbo.ScoringEntities", "BasePoints");
            DropColumn("dbo.ScoringEntities", "SeasonId");
            DropColumn("dbo.ScoringEntities", "Name");
            DropTable("dbo.Scoring_Session");
            DropTable("dbo.MultiScoringMap");
            CreateIndex("dbo.ScoringEntities", "ScheduleId");
            AddForeignKey("dbo.ScoringEntities", "ScheduleId", "dbo.ScheduleEntities", "ScheduleId");
        }
    }
}
