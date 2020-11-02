namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDateTimeColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ResultRowEntities", "QualifyingTimeAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResultRowEntities", "QualifyingTimeAt", c => c.DateTime(nullable: false));
        }
    }
}
