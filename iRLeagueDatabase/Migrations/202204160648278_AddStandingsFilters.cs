namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStandingsFilters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StandingsFilterOptionEntities",
                c => new
                    {
                        ResultsFilterId = c.Long(nullable: false, identity: true),
                        ScoringTableId = c.Long(nullable: false),
                        ResultsFilterType = c.String(),
                        ColumnPropertyName = c.String(),
                        Comparator = c.Int(nullable: false),
                        Exclude = c.Boolean(nullable: false),
                        FilterValues = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedByUserId = c.String(),
                        CreatedByUserName = c.String(),
                        LastModifiedByUserId = c.String(),
                        LastModifiedByUserName = c.String(),
                    })
                .PrimaryKey(t => t.ResultsFilterId)
                .ForeignKey("dbo.ScoringTableEntities", t => t.ScoringTableId, cascadeDelete: true)
                .Index(t => t.ScoringTableId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StandingsFilterOptionEntities", "ScoringTableId", "dbo.ScoringTableEntities");
            DropIndex("dbo.StandingsFilterOptionEntities", new[] { "ScoringTableId" });
            DropTable("dbo.StandingsFilterOptionEntities");
        }
    }
}
