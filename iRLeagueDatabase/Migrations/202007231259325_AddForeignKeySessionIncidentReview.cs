namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeySessionIncidentReview : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.IncidentReviewEntities", name: "Session_SessionId", newName: "SessionId");
            RenameIndex(table: "dbo.IncidentReviewEntities", name: "IX_Session_SessionId", newName: "IX_SessionId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.IncidentReviewEntities", name: "IX_SessionId", newName: "IX_Session_SessionId");
            RenameColumn(table: "dbo.IncidentReviewEntities", name: "SessionId", newName: "Session_SessionId");
        }
    }
}
