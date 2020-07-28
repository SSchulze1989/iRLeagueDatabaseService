namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeIncidentReviewAuthorIdentityToName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.IncidentReviewEntities", "Author_MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.IncidentReviewEntities", new[] { "Author_MemberId" });
            AddColumn("dbo.IncidentReviewEntities", "AuthorName", c => c.String());
            DropColumn("dbo.IncidentReviewEntities", "Author_MemberId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IncidentReviewEntities", "Author_MemberId", c => c.Long());
            DropColumn("dbo.IncidentReviewEntities", "AuthorName");
            CreateIndex("dbo.IncidentReviewEntities", "Author_MemberId");
            AddForeignKey("dbo.IncidentReviewEntities", "Author_MemberId", "dbo.LeagueMemberEntities", "MemberId");
        }
    }
}
