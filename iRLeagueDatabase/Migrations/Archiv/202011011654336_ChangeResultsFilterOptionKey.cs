namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeResultsFilterOptionKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ResultsFilterOptionEntities");
            AlterColumn("dbo.ResultsFilterOptionEntities", "ResultsFilterId", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.ResultsFilterOptionEntities", "ResultsFilterId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ResultsFilterOptionEntities");
            AlterColumn("dbo.ResultsFilterOptionEntities", "ResultsFilterId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.ResultsFilterOptionEntities", new[] { "ScoringId", "ResultsFilterId" });
        }
    }
}
