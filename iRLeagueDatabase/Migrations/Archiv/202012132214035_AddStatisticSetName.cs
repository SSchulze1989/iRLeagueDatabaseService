namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatisticSetName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatisticSetEntities", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatisticSetEntities", "Name");
        }
    }
}
