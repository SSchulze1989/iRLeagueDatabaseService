namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFilterPointsOnlyOption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultsFilterOptionEntities", "FilterPointsOnly", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResultsFilterOptionEntities", "FilterPointsOnly");
        }
    }
}
