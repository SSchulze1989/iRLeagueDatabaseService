namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddImportStatistics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatisticSetEntities", "RequiresRecalculation", c => c.Boolean(nullable: false));
            AddColumn("dbo.StatisticSetEntities", "ImportSource", c => c.String());
            AddColumn("dbo.StatisticSetEntities", "Description", c => c.String());
            AddColumn("dbo.StatisticSetEntities", "FirstDate", c => c.DateTime());
            AddColumn("dbo.StatisticSetEntities", "LastDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatisticSetEntities", "LastDate");
            DropColumn("dbo.StatisticSetEntities", "FirstDate");
            DropColumn("dbo.StatisticSetEntities", "Description");
            DropColumn("dbo.StatisticSetEntities", "ImportSource");
            DropColumn("dbo.StatisticSetEntities", "RequiresRecalculation");
        }
    }
}
