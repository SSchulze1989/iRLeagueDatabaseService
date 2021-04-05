namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPointsEligibleToResultRow : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResultRowEntities", "PointsEligible", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResultRowEntities", "PointsEligible");
        }
    }
}
