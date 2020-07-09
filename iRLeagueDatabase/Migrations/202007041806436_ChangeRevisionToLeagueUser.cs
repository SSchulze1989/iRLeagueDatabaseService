namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeRevisionToLeagueUser : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.SeasonEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.SeasonEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.ScoringEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.ScoringEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.ScheduleEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.ScheduleEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.SessionBaseEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.SessionBaseEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.ResultEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.ResultEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.ScoredResultEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.ScoredResultEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.IncidentReviewEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.IncidentReviewEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameColumn(table: "dbo.ReviewCommentEntities", name: "CreatedBy_MemberId", newName: "CreatedByUserId");
            RenameColumn(table: "dbo.ReviewCommentEntities", name: "LastModifiedBy_MemberId", newName: "LastModifiedByUserId");
            RenameIndex(table: "dbo.SeasonEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.SeasonEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.ScheduleEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.ScheduleEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.SessionBaseEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.SessionBaseEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.ResultEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.ResultEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.ScoredResultEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.ScoredResultEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.IncidentReviewEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.IncidentReviewEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
            RenameIndex(table: "dbo.ReviewCommentEntities", name: "IX_CreatedBy_MemberId", newName: "IX_CreatedByUserId");
            RenameIndex(table: "dbo.ReviewCommentEntities", name: "IX_LastModifiedBy_MemberId", newName: "IX_LastModifiedByUserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.ReviewCommentEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.ReviewCommentEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.IncidentReviewEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.IncidentReviewEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.ScoredResultEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.ScoredResultEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.ResultEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.ResultEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.SessionBaseEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.SessionBaseEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.ScheduleEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.ScheduleEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameIndex(table: "dbo.SeasonEntities", name: "IX_LastModifiedByUserId", newName: "IX_LastModifiedBy_MemberId");
            RenameIndex(table: "dbo.SeasonEntities", name: "IX_CreatedByUserId", newName: "IX_CreatedBy_MemberId");
            RenameColumn(table: "dbo.ReviewCommentEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.ReviewCommentEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.IncidentReviewEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.IncidentReviewEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.ScoredResultEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.ScoredResultEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.ResultEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.ResultEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.SessionBaseEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.SessionBaseEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.ScheduleEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.ScheduleEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.ScoringEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.ScoringEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
            RenameColumn(table: "dbo.SeasonEntities", name: "LastModifiedByUserId", newName: "LastModifiedBy_MemberId");
            RenameColumn(table: "dbo.SeasonEntities", name: "CreatedByUserId", newName: "CreatedBy_MemberId");
        }
    }
}
