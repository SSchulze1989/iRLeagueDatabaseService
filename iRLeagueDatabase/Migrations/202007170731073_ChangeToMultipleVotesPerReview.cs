namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeToMultipleVotesPerReview : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.IncidentReview_LeagueMember", newName: "IncidentReview_InvolvedLeagueMember");
            DropForeignKey("dbo.ReviewCommentEntities", "MemberAtFault_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "MemberAtFault_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ReviewCommentEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropIndex("dbo.IncidentReviewEntities", new[] { "MemberAtFault_MemberId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "ReviewId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "MemberAtFault_MemberId" });
            CreateTable(
                "dbo.AcceptedReviewVoteEntities",
                c => new
                    {
                        ReviewVoteId = c.Long(nullable: false, identity: true),
                        ReviewId = c.Long(nullable: false),
                        MemberAtFaultId = c.Long(nullable: false),
                        Vote = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewVoteId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberAtFaultId, cascadeDelete: true)
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReviewId)
                .Index(t => t.MemberAtFaultId);
            
            CreateTable(
                "dbo.CommentReviewVoteEntities",
                c => new
                    {
                        ReviewVoteId = c.Long(nullable: false, identity: true),
                        CommentId = c.Long(nullable: false),
                        MemberAtFaultId = c.Long(nullable: false),
                        Vote = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewVoteId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberAtFaultId, cascadeDelete: true)
                .ForeignKey("dbo.ReviewCommentEntities", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.CommentId)
                .Index(t => t.MemberAtFaultId);
            
            AlterColumn("dbo.ReviewCommentEntities", "ReviewId", c => c.Long(nullable: false));
            CreateIndex("dbo.ReviewCommentEntities", "ReviewId");
            AddForeignKey("dbo.ReviewCommentEntities", "ReviewId", "dbo.IncidentReviewEntities", "ReviewId", cascadeDelete: true);
            DropColumn("dbo.IncidentReviewEntities", "VoteResult");
            DropColumn("dbo.IncidentReviewEntities", "VoteState");
            DropColumn("dbo.IncidentReviewEntities", "MemberAtFault_MemberId");
            DropColumn("dbo.ReviewCommentEntities", "Vote");
            DropColumn("dbo.ReviewCommentEntities", "MemberAtFault_MemberId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReviewCommentEntities", "MemberAtFault_MemberId", c => c.Long());
            AddColumn("dbo.ReviewCommentEntities", "Vote", c => c.Int(nullable: false));
            AddColumn("dbo.IncidentReviewEntities", "MemberAtFault_MemberId", c => c.Long());
            AddColumn("dbo.IncidentReviewEntities", "VoteState", c => c.Int(nullable: false));
            AddColumn("dbo.IncidentReviewEntities", "VoteResult", c => c.Int(nullable: false));
            DropForeignKey("dbo.ReviewCommentEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.CommentReviewVoteEntities", "CommentId", "dbo.ReviewCommentEntities");
            DropForeignKey("dbo.CommentReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "MemberAtFaultId" });
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "CommentId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "ReviewId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "MemberAtFaultId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "ReviewId" });
            AlterColumn("dbo.ReviewCommentEntities", "ReviewId", c => c.Long());
            DropTable("dbo.CommentReviewVoteEntities");
            DropTable("dbo.AcceptedReviewVoteEntities");
            CreateIndex("dbo.ReviewCommentEntities", "MemberAtFault_MemberId");
            CreateIndex("dbo.ReviewCommentEntities", "ReviewId");
            CreateIndex("dbo.IncidentReviewEntities", "MemberAtFault_MemberId");
            AddForeignKey("dbo.ReviewCommentEntities", "ReviewId", "dbo.IncidentReviewEntities", "ReviewId");
            AddForeignKey("dbo.IncidentReviewEntities", "MemberAtFault_MemberId", "dbo.LeagueMemberEntities", "MemberId");
            AddForeignKey("dbo.ReviewCommentEntities", "MemberAtFault_MemberId", "dbo.LeagueMemberEntities", "MemberId");
            RenameTable(name: "dbo.IncidentReview_InvolvedLeagueMember", newName: "IncidentReview_LeagueMember");
        }
    }
}
