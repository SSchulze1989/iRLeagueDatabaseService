namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLapCornerString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IncidentReviewEntities", "OnLap", c => c.String());
            AlterColumn("dbo.IncidentReviewEntities", "Corner", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.IncidentReviewEntities", "Corner", c => c.Int(nullable: false));
            AlterColumn("dbo.IncidentReviewEntities", "OnLap", c => c.Int(nullable: false));
        }
    }
}
