namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequireCalculationOption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultEntities", "RequiresRecalculation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResultEntities", "RequiresRecalculation");
        }
    }
}
