namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultFilterOptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResultsFilterOptionEntities",
                c => new
                    {
                        ScoringId = c.Long(nullable: false),
                        ResultsFilterId = c.Long(nullable: false),
                        ResultsFilterType = c.String(),
                    })
                .PrimaryKey(t => new { t.ScoringId, t.ResultsFilterId })
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringId, cascadeDelete: true)
                .Index(t => t.ScoringId);
            
            CreateTable(
                "dbo.FilterValueBaseEntities",
                c => new
                    {
                        ScoringId = c.Long(nullable: false),
                        ResultsFilterOptionId = c.Long(nullable: false),
                        FilterValueId = c.Long(nullable: false),
                        DoubleValue = c.Double(),
                        IntValue = c.Int(),
                        MemberIdValue = c.Long(),
                        StringValue = c.String(),
                        TeamIdValue = c.Long(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ScoringId, t.ResultsFilterOptionId, t.FilterValueId })
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberIdValue, cascadeDelete: true)
                .ForeignKey("dbo.TeamEntities", t => t.TeamIdValue, cascadeDelete: true)
                .ForeignKey("dbo.ResultsFilterOptionEntities", t => new { t.ScoringId, t.ResultsFilterOptionId }, cascadeDelete: true)
                .Index(t => new { t.ScoringId, t.ResultsFilterOptionId })
                .Index(t => t.MemberIdValue)
                .Index(t => t.TeamIdValue);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultsFilterOptionEntities", "ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.FilterValueBaseEntities", new[] { "ScoringId", "ResultsFilterOptionId" }, "dbo.ResultsFilterOptionEntities");
            DropForeignKey("dbo.FilterValueBaseEntities", "TeamIdValue", "dbo.TeamEntities");
            DropForeignKey("dbo.FilterValueBaseEntities", "MemberIdValue", "dbo.LeagueMemberEntities");
            DropIndex("dbo.FilterValueBaseEntities", new[] { "TeamIdValue" });
            DropIndex("dbo.FilterValueBaseEntities", new[] { "MemberIdValue" });
            DropIndex("dbo.FilterValueBaseEntities", new[] { "ScoringId", "ResultsFilterOptionId" });
            DropIndex("dbo.ResultsFilterOptionEntities", new[] { "ScoringId" });
            DropTable("dbo.FilterValueBaseEntities");
            DropTable("dbo.ResultsFilterOptionEntities");
        }
    }
}
