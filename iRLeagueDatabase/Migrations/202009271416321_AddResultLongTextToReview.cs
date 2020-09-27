namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResultLongTextToReview : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncidentReviewEntities", "ResultLongText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.IncidentReviewEntities", "ResultLongText");
        }
    }
}
