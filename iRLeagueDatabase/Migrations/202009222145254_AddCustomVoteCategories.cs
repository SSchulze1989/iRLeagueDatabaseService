namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomVoteCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomIncidentEntities",
                c => new
                    {
                        IncidentId = c.Long(nullable: false, identity: true),
                        Text = c.String(),
                        Index = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IncidentId);
            
            CreateTable(
                "dbo.VoteCategoryEntities",
                c => new
                    {
                        CatId = c.Long(nullable: false, identity: true),
                        Text = c.String(),
                        Index = c.Int(nullable: false),
                        DefaultPenalty = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CatId);
            
            AddColumn("dbo.AcceptedReviewVoteEntities", "CustomVoteCatId", c => c.Long());
            AddColumn("dbo.CommentReviewVoteEntities", "CustomVoteCatId", c => c.Long());
            CreateIndex("dbo.AcceptedReviewVoteEntities", "CustomVoteCatId");
            CreateIndex("dbo.CommentReviewVoteEntities", "CustomVoteCatId");
            AddForeignKey("dbo.AcceptedReviewVoteEntities", "CustomVoteCatId", "dbo.VoteCategoryEntities", "CatId");
            AddForeignKey("dbo.CommentReviewVoteEntities", "CustomVoteCatId", "dbo.VoteCategoryEntities", "CatId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentReviewVoteEntities", "CustomVoteCatId", "dbo.VoteCategoryEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "CustomVoteCatId", "dbo.VoteCategoryEntities");
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "CustomVoteCatId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "CustomVoteCatId" });
            DropColumn("dbo.CommentReviewVoteEntities", "CustomVoteCatId");
            DropColumn("dbo.AcceptedReviewVoteEntities", "CustomVoteCatId");
            DropTable("dbo.VoteCategoryEntities");
            DropTable("dbo.CustomIncidentEntities");
        }
    }
}
