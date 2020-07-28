namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIncidentReviewDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncidentReviewEntities", "IncidentKind", c => c.String());
            AddColumn("dbo.IncidentReviewEntities", "FullDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IncidentReviewEntities", "FullDescription");
            DropColumn("dbo.IncidentReviewEntities", "IncidentKind");
        }
    }
}
