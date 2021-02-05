namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "UseResultSetTeam", c => c.Boolean(nullable: false));
            DropColumn("dbo.ScoringEntities", "TakeTeamFromResulSet");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoringEntities", "TakeTeamFromResulSet", c => c.Boolean(nullable: false));
            DropColumn("dbo.ScoringEntities", "UseResultSetTeam");
        }
    }
}
