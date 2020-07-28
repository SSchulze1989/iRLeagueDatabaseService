namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeCommentAuthorUserIdentityToName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReviewCommentEntities", "Author_MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.ReviewCommentEntities", new[] { "Author_MemberId" });
            AddColumn("dbo.ReviewCommentEntities", "AuthorName", c => c.String());
            DropColumn("dbo.ReviewCommentEntities", "Author_MemberId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReviewCommentEntities", "Author_MemberId", c => c.Long());
            DropColumn("dbo.ReviewCommentEntities", "AuthorName");
            CreateIndex("dbo.ReviewCommentEntities", "Author_MemberId");
            AddForeignKey("dbo.ReviewCommentEntities", "Author_MemberId", "dbo.LeagueMemberEntities", "MemberId");
        }
    }
}
