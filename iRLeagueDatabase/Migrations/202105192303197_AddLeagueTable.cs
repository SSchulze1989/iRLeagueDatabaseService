namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeagueTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeagueEntities",
                c => new
                    {
                        LeagueId = c.Long(nullable: false, identity: true),
                        LeagueName = c.String(),
                        LongName = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedByUserId = c.String(),
                        CreatedByUserName = c.String(),
                        LastModifiedByUserId = c.String(),
                        LastModifiedByUserName = c.String(),
                    })
                .PrimaryKey(t => t.LeagueId);
            Sql("INSERT INTO dbo.LeagueEntities (LeagueName, LongName, Version) VALUES ('Default', 'Default League', 0)");

            AddColumn("dbo.CustomIncidentEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.VoteCategoryEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.StatisticSetEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.LeagueMemberEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.TeamEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.SeasonEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.CommentBaseEntities", "LeagueId", c => c.Long(nullable: false));
            Sql("UPDATE dbo.VoteCategoryEntities SET LeagueId = 1");
            Sql("UPDATE dbo.LeagueMemberEntities SET LeagueId = 1");
            Sql("UPDATE dbo.TeamEntities SET LeagueId = 1");
            Sql("UPDATE dbo.CommentBaseEntities SET LeagueId = 1");
            Sql("UPDATE dbo.StatisticSetEntities SET LeagueId = 1");
            Sql("UPDATE dbo.SeasonEntities SET LeagueId = 1");
            Sql("UPDATE dbo.CustomIncidentEntities SET LeagueId = 1");
            CreateIndex("dbo.CustomIncidentEntities", "LeagueId");
            CreateIndex("dbo.SeasonEntities", "LeagueId");
            CreateIndex("dbo.VoteCategoryEntities", "LeagueId");
            CreateIndex("dbo.LeagueMemberEntities", "LeagueId");
            CreateIndex("dbo.TeamEntities", "LeagueId");
            CreateIndex("dbo.CommentBaseEntities", "LeagueId");
            CreateIndex("dbo.StatisticSetEntities", "LeagueId");
            AddForeignKey("dbo.VoteCategoryEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.LeagueMemberEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.TeamEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.CommentBaseEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.StatisticSetEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.SeasonEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.CustomIncidentEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomIncidentEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.SeasonEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.StatisticSetEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.CommentBaseEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.TeamEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.LeagueMemberEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.VoteCategoryEntities", "LeagueId", "dbo.LeagueEntities");
            DropIndex("dbo.StatisticSetEntities", new[] { "LeagueId" });
            DropIndex("dbo.CommentBaseEntities", new[] { "LeagueId" });
            DropIndex("dbo.TeamEntities", new[] { "LeagueId" });
            DropIndex("dbo.LeagueMemberEntities", new[] { "LeagueId" });
            DropIndex("dbo.VoteCategoryEntities", new[] { "LeagueId" });
            DropIndex("dbo.SeasonEntities", new[] { "LeagueId" });
            DropIndex("dbo.CustomIncidentEntities", new[] { "LeagueId" });
            DropColumn("dbo.CommentBaseEntities", "LeagueId");
            DropColumn("dbo.SeasonEntities", "LeagueId");
            DropColumn("dbo.TeamEntities", "LeagueId");
            DropColumn("dbo.LeagueMemberEntities", "LeagueId");
            DropColumn("dbo.StatisticSetEntities", "LeagueId");
            DropColumn("dbo.VoteCategoryEntities", "LeagueId");
            DropColumn("dbo.CustomIncidentEntities", "LeagueId");
            DropTable("dbo.LeagueEntities");
        }
    }
}
