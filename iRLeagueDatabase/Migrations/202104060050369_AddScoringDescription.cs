namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScoringDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoringEntities", "Description");
        }
    }
}
