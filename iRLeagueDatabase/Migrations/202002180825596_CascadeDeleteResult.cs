namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeleteResult : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResultEntities", "ResultId", "dbo.SessionBaseEntities");
            AddForeignKey("dbo.ResultEntities", "ResultId", "dbo.SessionBaseEntities", "SessionId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultEntities", "ResultId", "dbo.SessionBaseEntities");
            AddForeignKey("dbo.ResultEntities", "ResultId", "dbo.SessionBaseEntities", "SessionId");
        }
    }
}
