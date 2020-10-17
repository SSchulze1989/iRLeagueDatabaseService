namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_AddPenalty_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddPenaltyEntities",
                c => new
                    {
                        ScoredResultRowId = c.Long(nullable: false),
                        PenaltyPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScoredResultRowId)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.ScoredResultRowId)
                .Index(t => t.ScoredResultRowId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AddPenaltyEntities", "ScoredResultRowId", "dbo.ScoredResultRowEntities");
            DropIndex("dbo.AddPenaltyEntities", new[] { "ScoredResultRowId" });
            DropTable("dbo.AddPenaltyEntities");
        }
    }
}
