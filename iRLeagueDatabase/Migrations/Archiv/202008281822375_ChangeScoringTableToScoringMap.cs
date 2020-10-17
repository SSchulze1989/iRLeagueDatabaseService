namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeScoringTableToScoringMap : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId", "dbo.ScoringTableEntities");
            DropIndex("dbo.ScoringEntities", new[] { "ScoringTableEntity_ScoringTableId" });
            CreateTable(
                "dbo.ScoringTableMap",
                c => new
                    {
                        ScoringTableRefId = c.Long(nullable: false),
                        ScoringRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoringTableRefId, t.ScoringRefId })
                .ForeignKey("dbo.ScoringTableEntities", t => t.ScoringTableRefId, cascadeDelete: true)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringRefId, cascadeDelete: false)
                .Index(t => t.ScoringTableRefId)
                .Index(t => t.ScoringRefId);
            
            DropColumn("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId", c => c.Long());
            DropForeignKey("dbo.ScoringTableMap", "ScoringRefId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringTableMap", "ScoringTableRefId", "dbo.ScoringTableEntities");
            DropIndex("dbo.ScoringTableMap", new[] { "ScoringRefId" });
            DropIndex("dbo.ScoringTableMap", new[] { "ScoringTableRefId" });
            DropTable("dbo.ScoringTableMap");
            CreateIndex("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId");
            AddForeignKey("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId", "dbo.ScoringTableEntities", "ScoringTableId");
        }
    }
}
