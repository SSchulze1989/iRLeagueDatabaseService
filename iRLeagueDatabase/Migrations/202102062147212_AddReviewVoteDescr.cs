namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewVoteDescr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AcceptedReviewVoteEntities", "Description", c => c.String());
            AddColumn("dbo.CommentReviewVoteEntities", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommentReviewVoteEntities", "Description");
            DropColumn("dbo.AcceptedReviewVoteEntities", "Description");
        }
    }
}
