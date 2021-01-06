namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateIntervalToStatisticSet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StatisticSetEntities", "UpdateInterval", c => c.Long(nullable: false));
            AddColumn("dbo.StatisticSetEntities", "UpdateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.StatisticSetEntities", "UpdateTime");
            DropColumn("dbo.StatisticSetEntities", "UpdateInterval");
        }
    }
}
