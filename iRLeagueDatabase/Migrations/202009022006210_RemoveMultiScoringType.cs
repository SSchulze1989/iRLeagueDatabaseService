namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveMultiScoringType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MultiScoringMap", "ScoringParentId", "dbo.ScoringEntities");
            DropForeignKey("dbo.MultiScoringMap", "ScoringChildId", "dbo.ScoringEntities");
            DropIndex("dbo.MultiScoringMap", new[] { "ScoringParentId" });
            DropIndex("dbo.MultiScoringMap", new[] { "ScoringChildId" });
            DropColumn("dbo.ScoringEntities", "IsMultiScoring");
            DropColumn("dbo.ScoringEntities", "MultiScoringFactors");
            DropTable("dbo.MultiScoringMap");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MultiScoringMap",
                c => new
                    {
                        ScoringParentId = c.Long(nullable: false),
                        ScoringChildId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoringParentId, t.ScoringChildId });
            
            AddColumn("dbo.ScoringEntities", "MultiScoringFactors", c => c.String());
            AddColumn("dbo.ScoringEntities", "IsMultiScoring", c => c.Boolean(nullable: false));
            CreateIndex("dbo.MultiScoringMap", "ScoringChildId");
            CreateIndex("dbo.MultiScoringMap", "ScoringParentId");
            AddForeignKey("dbo.MultiScoringMap", "ScoringChildId", "dbo.ScoringEntities", "ScoringId");
            AddForeignKey("dbo.MultiScoringMap", "ScoringParentId", "dbo.ScoringEntities", "ScoringId");
        }
    }
}
