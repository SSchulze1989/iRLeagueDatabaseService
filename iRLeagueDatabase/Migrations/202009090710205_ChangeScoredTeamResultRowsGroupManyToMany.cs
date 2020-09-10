namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeScoredTeamResultRowsGroupManyToMany : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ScoredResultRowEntities", "TeamResultRowId", "dbo.ScoredTeamResultRowEntities");
            DropIndex("dbo.ScoredResultRowEntities", new[] { "TeamResultRowId" });
            CreateTable(
                "dbo.ScoredTeamResultRowsGroup",
                c => new
                    {
                        ScoredTeamResultRowRefId = c.Long(nullable: false),
                        ScoredResultRowRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoredTeamResultRowRefId, t.ScoredResultRowRefId })
                .ForeignKey("dbo.ScoredTeamResultRowEntities", t => t.ScoredTeamResultRowRefId, cascadeDelete: true)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.ScoredResultRowRefId, cascadeDelete: false)
                .Index(t => t.ScoredTeamResultRowRefId)
                .Index(t => t.ScoredResultRowRefId);
            
            DropColumn("dbo.ScoredResultRowEntities", "TeamResultRowId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoredResultRowEntities", "TeamResultRowId", c => c.Long());
            DropForeignKey("dbo.ScoredTeamResultRowsGroup", "ScoredResultRowRefId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.ScoredTeamResultRowsGroup", "ScoredTeamResultRowRefId", "dbo.ScoredTeamResultRowEntities");
            DropIndex("dbo.ScoredTeamResultRowsGroup", new[] { "ScoredResultRowRefId" });
            DropIndex("dbo.ScoredTeamResultRowsGroup", new[] { "ScoredTeamResultRowRefId" });
            DropTable("dbo.ScoredTeamResultRowsGroup");
            CreateIndex("dbo.ScoredResultRowEntities", "TeamResultRowId");
            AddForeignKey("dbo.ScoredResultRowEntities", "TeamResultRowId", "dbo.ScoredTeamResultRowEntities", "ScoredResultRowId");
        }
    }
}
