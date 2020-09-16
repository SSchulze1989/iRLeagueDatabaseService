namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeMemberAtFaultOptional : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.CommentReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "MemberAtFaultId" });
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "MemberAtFaultId" });
            AlterColumn("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", c => c.Long());
            AlterColumn("dbo.CommentReviewVoteEntities", "MemberAtFaultId", c => c.Long());
            CreateIndex("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId");
            CreateIndex("dbo.CommentReviewVoteEntities", "MemberAtFaultId");
            AddForeignKey("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities", "MemberId");
            AddForeignKey("dbo.CommentReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities", "MemberId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "MemberAtFaultId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "MemberAtFaultId" });
            AlterColumn("dbo.CommentReviewVoteEntities", "MemberAtFaultId", c => c.Long(nullable: false));
            AlterColumn("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", c => c.Long(nullable: false));
            CreateIndex("dbo.CommentReviewVoteEntities", "MemberAtFaultId");
            CreateIndex("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId");
            AddForeignKey("dbo.CommentReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities", "MemberId", cascadeDelete: false);
            AddForeignKey("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities", "MemberId", cascadeDelete: false);
        }
    }
}
