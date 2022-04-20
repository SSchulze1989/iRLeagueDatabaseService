namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimePenalties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoredResultRowEntities", "PenaltyTime", c => c.Long(nullable: false));
            AddColumn("dbo.AddPenaltyEntities", "PenaltyTime", c => c.Long(nullable: false));
            AddColumn("dbo.ReviewPenaltyEntities", "PenaltyTime", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReviewPenaltyEntities", "PenaltyTime");
            DropColumn("dbo.AddPenaltyEntities", "PenaltyTime");
            DropColumn("dbo.ScoredResultRowEntities", "PenaltyTime");
        }
    }
}
