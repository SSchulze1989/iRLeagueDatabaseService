namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompletedPctColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultRowEntities", "CompletedPct", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResultRowEntities", "CompletedPct");
        }
    }
}
