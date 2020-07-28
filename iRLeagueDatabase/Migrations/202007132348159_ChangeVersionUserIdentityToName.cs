namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVersionUserIdentityToName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeasonEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.SeasonEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.ScoringEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.ScoringEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.ScheduleEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.ScheduleEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.SessionBaseEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.SessionBaseEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.ResultEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.ResultEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.ScoredResultEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.ScoredResultEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.IncidentReviewEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.IncidentReviewEntities", "LastModifiedByUserName", c => c.String());
            AddColumn("dbo.ReviewCommentEntities", "CreatedByUserName", c => c.String());
            AddColumn("dbo.ReviewCommentEntities", "LastModifiedByUserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReviewCommentEntities", "LastModifiedByUserName");
            DropColumn("dbo.ReviewCommentEntities", "CreatedByUserName");
            DropColumn("dbo.IncidentReviewEntities", "LastModifiedByUserName");
            DropColumn("dbo.IncidentReviewEntities", "CreatedByUserName");
            DropColumn("dbo.ScoredResultEntities", "LastModifiedByUserName");
            DropColumn("dbo.ScoredResultEntities", "CreatedByUserName");
            DropColumn("dbo.ResultEntities", "LastModifiedByUserName");
            DropColumn("dbo.ResultEntities", "CreatedByUserName");
            DropColumn("dbo.SessionBaseEntities", "LastModifiedByUserName");
            DropColumn("dbo.SessionBaseEntities", "CreatedByUserName");
            DropColumn("dbo.ScheduleEntities", "LastModifiedByUserName");
            DropColumn("dbo.ScheduleEntities", "CreatedByUserName");
            DropColumn("dbo.ScoringEntities", "LastModifiedByUserName");
            DropColumn("dbo.ScoringEntities", "CreatedByUserName");
            DropColumn("dbo.SeasonEntities", "LastModifiedByUserName");
            DropColumn("dbo.SeasonEntities", "CreatedByUserName");
        }
    }
}
