namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOldNewSafetyRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultRowEntities", "OldSafetyRating", c => c.Double(nullable: false));
            AddColumn("dbo.ResultRowEntities", "NewSafetyRating", c => c.Double(nullable: false));
            DropColumn("dbo.ResultRowEntities", "SafetyRating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResultRowEntities", "SafetyRating", c => c.Double(nullable: false));
            DropColumn("dbo.ResultRowEntities", "NewSafetyRating");
            DropColumn("dbo.ResultRowEntities", "OldSafetyRating");
        }
    }
}
