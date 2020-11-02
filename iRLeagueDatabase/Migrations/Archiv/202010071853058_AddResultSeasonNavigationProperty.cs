namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultSeasonNavigationProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultEntities", "SeasonId", c => c.Long());
            CreateIndex("dbo.ResultEntities", "SeasonId");
            AddForeignKey("dbo.ResultEntities", "SeasonId", "dbo.SeasonEntities", "SeasonId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultEntities", "SeasonId", "dbo.SeasonEntities");
            DropIndex("dbo.ResultEntities", new[] { "SeasonId" });
            DropColumn("dbo.ResultEntities", "SeasonId");
        }
    }
}
