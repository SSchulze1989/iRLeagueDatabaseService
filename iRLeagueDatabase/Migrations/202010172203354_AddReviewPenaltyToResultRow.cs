namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReviewPenaltyToResultRow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReviewPenaltyEntities", "ReviewVoteId", c => c.Long());
            CreateIndex("dbo.ReviewPenaltyEntities", "ReviewVoteId");
            AddForeignKey("dbo.ReviewPenaltyEntities", "ReviewVoteId", "dbo.AcceptedReviewVoteEntities", "ReviewVoteId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReviewPenaltyEntities", "ReviewVoteId", "dbo.AcceptedReviewVoteEntities");
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "ReviewVoteId" });
            DropColumn("dbo.ReviewPenaltyEntities", "ReviewVoteId");
        }
    }
}
