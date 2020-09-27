namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHideCommentsBeforeVotedOption : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeasonEntities", "HideCommentsBeforeVoted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SeasonEntities", "HideCommentsBeforeVoted");
        }
    }
}
