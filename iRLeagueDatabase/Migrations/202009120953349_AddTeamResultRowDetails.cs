namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeamResultRowDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoredTeamResultRowEntities", "Date", c => c.DateTime());
            AddColumn("dbo.ScoredTeamResultRowEntities", "ClassId", c => c.Int(nullable: false));
            AddColumn("dbo.ScoredTeamResultRowEntities", "CarClass", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoredTeamResultRowEntities", "CarClass");
            DropColumn("dbo.ScoredTeamResultRowEntities", "ClassId");
            DropColumn("dbo.ScoredTeamResultRowEntities", "Date");
        }
    }
}
