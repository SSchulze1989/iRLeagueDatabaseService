namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTotalPointsCol_ScoredResultRow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoredResultRowEntities", "TotalPoints", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoredResultRowEntities", "TotalPoints");
        }
    }
}
