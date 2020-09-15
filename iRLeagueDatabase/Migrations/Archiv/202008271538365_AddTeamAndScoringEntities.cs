namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeamAndScoringEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamEntities",
                c => new
                    {
                        TeamId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Profile = c.String(),
                        TeamColor = c.String(),
                        TeamHomepage = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedByUserId = c.String(),
                        CreatedByUserName = c.String(),
                        LastModifiedByUserId = c.String(),
                        LastModifiedByUserName = c.String(),
                    })
                .PrimaryKey(t => t.TeamId);
            
            CreateTable(
                "dbo.ScoringTableEntities",
                c => new
                    {
                        ScoringTableId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        DropWeeks = c.Int(nullable: false),
                        AverageRaceNr = c.Int(nullable: false),
                        ScoringFactors = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedByUserId = c.String(),
                        CreatedByUserName = c.String(),
                        LastModifiedByUserId = c.String(),
                        LastModifiedByUserName = c.String(),
                        Season_SeasonId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ScoringTableId)
                .ForeignKey("dbo.SeasonEntities", t => t.Season_SeasonId, cascadeDelete: true)
                .Index(t => t.Season_SeasonId);
            
            AddColumn("dbo.LeagueMemberEntities", "TeamId", c => c.Long());
            AddColumn("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId", c => c.Long());
            CreateIndex("dbo.LeagueMemberEntities", "TeamId");
            CreateIndex("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId");
            AddForeignKey("dbo.LeagueMemberEntities", "TeamId", "dbo.TeamEntities", "TeamId");
            AddForeignKey("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId", "dbo.ScoringTableEntities", "ScoringTableId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoringTableEntities", "Season_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId", "dbo.ScoringTableEntities");
            DropForeignKey("dbo.LeagueMemberEntities", "TeamId", "dbo.TeamEntities");
            DropIndex("dbo.ScoringTableEntities", new[] { "Season_SeasonId" });
            DropIndex("dbo.ScoringEntities", new[] { "ScoringTableEntity_ScoringTableId" });
            DropIndex("dbo.LeagueMemberEntities", new[] { "TeamId" });
            DropColumn("dbo.ScoringEntities", "ScoringTableEntity_ScoringTableId");
            DropColumn("dbo.LeagueMemberEntities", "TeamId");
            DropTable("dbo.ScoringTableEntities");
            DropTable("dbo.TeamEntities");
        }
    }
}
