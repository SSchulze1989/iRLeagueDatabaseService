namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveFilterValueEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FilterValueBaseEntities", "MemberIdValue", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.FilterValueBaseEntities", "TeamIdValue", "dbo.TeamEntities");
            DropForeignKey("dbo.FilterValueBaseEntities", new[] { "ScoringId", "ResultsFilterOptionId" }, "dbo.ResultsFilterOptionEntities");
            DropIndex("dbo.FilterValueBaseEntities", new[] { "ScoringId", "ResultsFilterOptionId" });
            DropIndex("dbo.FilterValueBaseEntities", new[] { "MemberIdValue" });
            DropIndex("dbo.FilterValueBaseEntities", new[] { "TeamIdValue" });
            AddColumn("dbo.ResultsFilterOptionEntities", "ColumnPropertyName", c => c.String());
            AddColumn("dbo.ResultsFilterOptionEntities", "Comparator", c => c.Int(nullable: false));
            AddColumn("dbo.ResultsFilterOptionEntities", "FilterValues", c => c.String());
            DropTable("dbo.FilterValueBaseEntities");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => new { t.ScoringId, t.ResultsFilterOptionId, t.FilterValueId });
            
            DropColumn("dbo.ResultsFilterOptionEntities", "FilterValues");
            DropColumn("dbo.ResultsFilterOptionEntities", "Comparator");
            DropColumn("dbo.ResultsFilterOptionEntities", "ColumnPropertyName");
            CreateIndex("dbo.FilterValueBaseEntities", "TeamIdValue");
            CreateIndex("dbo.FilterValueBaseEntities", "MemberIdValue");
            CreateIndex("dbo.FilterValueBaseEntities", new[] { "ScoringId", "ResultsFilterOptionId" });
            AddForeignKey("dbo.FilterValueBaseEntities", new[] { "ScoringId", "ResultsFilterOptionId" }, "dbo.ResultsFilterOptionEntities", new[] { "ScoringId", "ResultsFilterId" }, cascadeDelete: true);
            AddForeignKey("dbo.FilterValueBaseEntities", "TeamIdValue", "dbo.TeamEntities", "TeamId", cascadeDelete: true);
            AddForeignKey("dbo.FilterValueBaseEntities", "MemberIdValue", "dbo.LeagueMemberEntities", "MemberId", cascadeDelete: true);
        }
    }
}
