namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIncidentNr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncidentReviewEntities", "IncidentNr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IncidentReviewEntities", "IncidentNr");
        }
    }
}
