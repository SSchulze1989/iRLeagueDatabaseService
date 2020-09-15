namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRevisionUserIdentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SeasonEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.SeasonEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ScheduleEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ScheduleEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.SessionBaseEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.SessionBaseEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.CommentBaseEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.CommentBaseEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ResultEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ScoredResultEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ScoredResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ScoringEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities");
            DropForeignKey("dbo.ScoringEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities");
            DropIndex("dbo.SeasonEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.SeasonEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ScoringEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ScoringEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ScheduleEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ScheduleEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.CommentBaseEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.CommentBaseEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ResultEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ResultEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "LastModifiedBy_MemberId" });
            DropColumn("dbo.SeasonEntities", "CreatedBy_MemberId");
            DropColumn("dbo.SeasonEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.ScoringEntities", "CreatedBy_MemberId");
            DropColumn("dbo.ScoringEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.ScheduleEntities", "CreatedBy_MemberId");
            DropColumn("dbo.ScheduleEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.SessionBaseEntities", "CreatedBy_MemberId");
            DropColumn("dbo.SessionBaseEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.IncidentReviewEntities", "CreatedBy_MemberId");
            DropColumn("dbo.IncidentReviewEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.CommentBaseEntities", "CreatedBy_MemberId");
            DropColumn("dbo.CommentBaseEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.ResultEntities", "CreatedBy_MemberId");
            DropColumn("dbo.ResultEntities", "LastModifiedBy_MemberId");
            DropColumn("dbo.ScoredResultEntities", "CreatedBy_MemberId");
            DropColumn("dbo.ScoredResultEntities", "LastModifiedBy_MemberId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ScoredResultEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ResultEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ResultEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.CommentBaseEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.CommentBaseEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.IncidentReviewEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.IncidentReviewEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.SessionBaseEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.SessionBaseEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ScheduleEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ScheduleEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoringEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoringEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.SeasonEntities", "LastModifiedBy_MemberId", c => c.Long(nullable: false));
            AddColumn("dbo.SeasonEntities", "CreatedBy_MemberId", c => c.Long(nullable: false));
            CreateIndex("dbo.ScoredResultEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.ScoredResultEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.ResultEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.ResultEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.CommentBaseEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.CommentBaseEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.IncidentReviewEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.IncidentReviewEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.SessionBaseEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.SessionBaseEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.ScheduleEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.ScheduleEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.ScoringEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.ScoringEntities", "CreatedBy_MemberId");
            CreateIndex("dbo.SeasonEntities", "LastModifiedBy_MemberId");
            CreateIndex("dbo.SeasonEntities", "CreatedBy_MemberId");
            AddForeignKey("dbo.ScoringEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ScoringEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ScoredResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ScoredResultEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ResultEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.IncidentReviewEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.IncidentReviewEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.CommentBaseEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.CommentBaseEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.SessionBaseEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.SessionBaseEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ScheduleEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.ScheduleEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.SeasonEntities", "LastModifiedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
            AddForeignKey("dbo.SeasonEntities", "CreatedBy_MemberId", "dbo.LeagueUserEntities", "AdminId", cascadeDelete: true);
        }
    }
}
