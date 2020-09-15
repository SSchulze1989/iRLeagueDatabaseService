namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRevisionUserIdentityUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeasonEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.SeasonEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.ScoringEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.ScoringEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.ScheduleEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.ScheduleEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.SessionBaseEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.SessionBaseEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.IncidentReviewEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.IncidentReviewEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.CommentBaseEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.CommentBaseEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.ResultEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.ResultEntities", "LastModifiedByUserId", c => c.String());
            AddColumn("dbo.ScoredResultEntities", "CreatedByUserId", c => c.String());
            AddColumn("dbo.ScoredResultEntities", "LastModifiedByUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScoredResultEntities", "LastModifiedByUserId");
            DropColumn("dbo.ScoredResultEntities", "CreatedByUserId");
            DropColumn("dbo.ResultEntities", "LastModifiedByUserId");
            DropColumn("dbo.ResultEntities", "CreatedByUserId");
            DropColumn("dbo.CommentBaseEntities", "LastModifiedByUserId");
            DropColumn("dbo.CommentBaseEntities", "CreatedByUserId");
            DropColumn("dbo.IncidentReviewEntities", "LastModifiedByUserId");
            DropColumn("dbo.IncidentReviewEntities", "CreatedByUserId");
            DropColumn("dbo.SessionBaseEntities", "LastModifiedByUserId");
            DropColumn("dbo.SessionBaseEntities", "CreatedByUserId");
            DropColumn("dbo.ScheduleEntities", "LastModifiedByUserId");
            DropColumn("dbo.ScheduleEntities", "CreatedByUserId");
            DropColumn("dbo.ScoringEntities", "LastModifiedByUserId");
            DropColumn("dbo.ScoringEntities", "CreatedByUserId");
            DropColumn("dbo.SeasonEntities", "LastModifiedByUserId");
            DropColumn("dbo.SeasonEntities", "CreatedByUserId");
        }
    }
}
