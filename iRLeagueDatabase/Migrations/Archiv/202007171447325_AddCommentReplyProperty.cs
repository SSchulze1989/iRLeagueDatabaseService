namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentReplyProperty : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ReviewCommentEntities", newName: "CommentBaseEntities");
            DropIndex("dbo.CommentBaseEntities", new[] { "ReviewId" });
            AddColumn("dbo.CommentBaseEntities", "ReplyToCommentId", c => c.Long());
            AddColumn("dbo.CommentBaseEntities", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.CommentBaseEntities", "ReviewId", c => c.Long());
            CreateIndex("dbo.CommentBaseEntities", "ReplyToCommentId");
            CreateIndex("dbo.CommentBaseEntities", "ReviewId");
            AddForeignKey("dbo.CommentBaseEntities", "ReplyToCommentId", "dbo.CommentBaseEntities", "CommentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentBaseEntities", "ReplyToCommentId", "dbo.CommentBaseEntities");
            DropIndex("dbo.CommentBaseEntities", new[] { "ReviewId" });
            DropIndex("dbo.CommentBaseEntities", new[] { "ReplyToCommentId" });
            AlterColumn("dbo.CommentBaseEntities", "ReviewId", c => c.Long(nullable: false));
            DropColumn("dbo.CommentBaseEntities", "Discriminator");
            DropColumn("dbo.CommentBaseEntities", "ReplyToCommentId");
            CreateIndex("dbo.CommentBaseEntities", "ReviewId");
            RenameTable(name: "dbo.CommentBaseEntities", newName: "ReviewCommentEntities");
        }
    }
}
