namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeagueMemberEntities",
                c => new
                    {
                        MemberId = c.Long(nullable: false, identity: true),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        IRacingId = c.String(),
                        DanLisaId = c.String(),
                        DiscordId = c.String(),
                    })
                .PrimaryKey(t => t.MemberId);
            
            CreateTable(
                "dbo.SeasonEntities",
                c => new
                    {
                        SeasonId = c.Long(nullable: false, identity: true),
                        SeasonName = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                        MainScoring_ScoringId = c.Long(),
                    })
                .PrimaryKey(t => t.SeasonId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.ScoringEntities", t => t.MainScoring_ScoringId)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId)
                .Index(t => t.MainScoring_ScoringId);
            
            CreateTable(
                "dbo.ScoringEntities",
                c => new
                    {
                        ScoringId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        DropWeeks = c.Int(nullable: false),
                        AverageRaceNr = c.Int(nullable: false),
                        BasePoints = c.String(),
                        BonusPoints = c.String(),
                        IncPenaltyPoints = c.String(),
                        MultiScoringFactors = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        ConnectedSchedule_ScheduleId = c.Long(),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                        Season_SeasonId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ScoringId)
                .ForeignKey("dbo.ScheduleEntities", t => t.ConnectedSchedule_ScheduleId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.SeasonEntities", t => t.Season_SeasonId, cascadeDelete: false)
                .Index(t => t.ConnectedSchedule_ScheduleId)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId)
                .Index(t => t.Season_SeasonId);
            
            CreateTable(
                "dbo.ScheduleEntities",
                c => new
                    {
                        ScheduleId = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                        Season_SeasonId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ScheduleId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.SeasonEntities", t => t.Season_SeasonId, cascadeDelete: false)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId)
                .Index(t => t.Season_SeasonId);
            
            CreateTable(
                "dbo.SessionBaseEntities",
                c => new
                    {
                        SessionId = c.Long(nullable: false, identity: true),
                        SessionTitle = c.String(),
                        SessionType = c.Int(nullable: false),
                        Date = c.DateTime(),
                        LocationId = c.String(),
                        Duration = c.Time(nullable: false, precision: 7),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        RaceId = c.Long(),
                        Laps = c.Int(),
                        PracticeLength = c.Time(precision: 7),
                        QualyLength = c.Time(precision: 7),
                        RaceLength = c.Time(precision: 7),
                        IrSessionId = c.String(),
                        IrResultLink = c.String(),
                        QualyAttached = c.Boolean(),
                        PracticeAttached = c.Boolean(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                        Schedule_ScheduleId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.SessionId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.ScheduleEntities", t => t.Schedule_ScheduleId, cascadeDelete: false)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId)
                .Index(t => t.Schedule_ScheduleId);
            
            CreateTable(
                "dbo.ResultEntities",
                c => new
                    {
                        ResultId = c.Long(nullable: false),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                    })
                .PrimaryKey(t => t.ResultId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.ResultId, cascadeDelete: false)
                .Index(t => t.ResultId)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId);
            
            CreateTable(
                "dbo.ResultRowEntities",
                c => new
                    {
                        ResultRowId = c.Long(nullable: false, identity: true),
                        ResultId = c.Long(nullable: false),
                        StartPosition = c.Int(nullable: false),
                        FinishPosition = c.Int(nullable: false),
                        CarNumber = c.Int(nullable: false),
                        ClassId = c.Int(nullable: false),
                        Car = c.String(),
                        CarClass = c.String(),
                        CompletedLaps = c.Int(nullable: false),
                        LeadLaps = c.Int(nullable: false),
                        FastLapNr = c.Int(nullable: false),
                        Incidents = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        QualifyingTime = c.Long(nullable: false),
                        Interval = c.Long(nullable: false),
                        AvgLapTime = c.Long(nullable: false),
                        FastestLapTime = c.Long(nullable: false),
                        PositionChange = c.Int(nullable: false),
                        IRacingId = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Member_MemberId = c.Long(),
                    })
                .PrimaryKey(t => new { t.ResultRowId, t.ResultId })
                .ForeignKey("dbo.LeagueMemberEntities", t => t.Member_MemberId)
                .ForeignKey("dbo.ResultEntities", t => t.ResultId, cascadeDelete: false)
                .Index(t => t.ResultId)
                .Index(t => t.Member_MemberId);
            
            CreateTable(
                "dbo.IncidentReviewEntities",
                c => new
                    {
                        ReviewId = c.Long(nullable: false, identity: true),
                        OnLap = c.Int(nullable: false),
                        Corner = c.Int(nullable: false),
                        TimeStamp = c.Time(nullable: false, precision: 7),
                        VoteResult = c.Int(nullable: false),
                        VoteState = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        Author_MemberId = c.Long(),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                        MemberAtFault_MemberId = c.Long(),
                        Result_ResultId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.Author_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberAtFault_MemberId)
                .ForeignKey("dbo.ResultEntities", t => t.Result_ResultId, cascadeDelete: false)
                .Index(t => t.Author_MemberId)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId)
                .Index(t => t.MemberAtFault_MemberId)
                .Index(t => t.Result_ResultId);
            
            CreateTable(
                "dbo.ReviewCommentEntities",
                c => new
                    {
                        CommentId = c.Long(nullable: false, identity: true),
                        Vote = c.Int(nullable: false),
                        ReviewId = c.Long(),
                        Date = c.DateTime(),
                        Text = c.String(),
                        CreatedOn = c.DateTime(),
                        LastModifiedOn = c.DateTime(),
                        Version = c.Int(nullable: false),
                        Author_MemberId = c.Long(),
                        CreatedBy_MemberId = c.Long(),
                        LastModifiedBy_MemberId = c.Long(),
                        MemberAtFault_MemberId = c.Long(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.Author_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.CreatedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.LastModifiedBy_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberAtFault_MemberId)
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewId)
                .Index(t => t.ReviewId)
                .Index(t => t.Author_MemberId)
                .Index(t => t.CreatedBy_MemberId)
                .Index(t => t.LastModifiedBy_MemberId)
                .Index(t => t.MemberAtFault_MemberId);
            
            CreateTable(
                "dbo.ScoredResultRowEntities",
                c => new
                    {
                        ResultRowId = c.Long(nullable: false),
                        ResultId = c.Long(nullable: false),
                        ScoredResultId = c.Long(nullable: false),
                        ScoringId = c.Long(nullable: false),
                        RacePoints = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        PenaltyPoints = c.Int(nullable: false),
                        FinalPosition = c.Int(nullable: false),
                        FinalPositionChange = c.Int(nullable: false),
                        TotalPoints = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResultRowId, t.ResultId, t.ScoredResultId, t.ScoringId })
                .ForeignKey("dbo.ResultRowEntities", t => new { t.ResultRowId, t.ResultId }, cascadeDelete: false)
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ScoredResultId, t.ScoringId }, cascadeDelete: false)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringId, cascadeDelete: false)
                .Index(t => new { t.ResultRowId, t.ResultId })
                .Index(t => new { t.ScoredResultId, t.ScoringId });
            
            CreateTable(
                "dbo.ScoredResultEntities",
                c => new
                    {
                        ResultId = c.Long(nullable: false),
                        ScoringId = c.Long(nullable: false),
                        FastestLapDriver_MemberId = c.Long(),
                    })
                .PrimaryKey(t => new { t.ResultId, t.ScoringId })
                .ForeignKey("dbo.LeagueMemberEntities", t => t.FastestLapDriver_MemberId)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringId, cascadeDelete: false)
                .ForeignKey("dbo.SessionBaseEntities", t => t.ResultId, cascadeDelete: false)
                .Index(t => t.ResultId)
                .Index(t => t.ScoringId)
                .Index(t => t.FastestLapDriver_MemberId);
            
            CreateTable(
                "dbo.IncidentReview_LeagueMember",
                c => new
                    {
                        ReviewRefId = c.Long(nullable: false),
                        MemberRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ReviewRefId, t.MemberRefId })
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewRefId, cascadeDelete: false)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberRefId, cascadeDelete: false)
                .Index(t => t.ReviewRefId)
                .Index(t => t.MemberRefId);
            
            CreateTable(
                "dbo.MultiScoringMap",
                c => new
                    {
                        ScoringParentId = c.Long(nullable: false),
                        ScoringChildId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoringParentId, t.ScoringChildId })
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringParentId)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringChildId)
                .Index(t => t.ScoringParentId)
                .Index(t => t.ScoringChildId);
            
            CreateTable(
                "dbo.Scoring_Session",
                c => new
                    {
                        ScoringRefId = c.Long(nullable: false),
                        SessionRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ScoringRefId, t.SessionRefId })
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringRefId, cascadeDelete: false)
                .ForeignKey("dbo.SessionBaseEntities", t => t.SessionRefId, cascadeDelete: false)
                .Index(t => t.ScoringRefId)
                .Index(t => t.SessionRefId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScheduleEntities", "Season_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.SeasonEntities", "MainScoring_ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.Scoring_Session", "SessionRefId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.Scoring_Session", "ScoringRefId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringEntities", "Season_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", "ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoredResultEntities", "ResultId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.ScoredResultEntities", "ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredResultEntities", "FastestLapDriver_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" }, "dbo.ResultRowEntities");
            DropForeignKey("dbo.MultiScoringMap", "ScoringChildId", "dbo.ScoringEntities");
            DropForeignKey("dbo.MultiScoringMap", "ScoringParentId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoringEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoringEntities", "ConnectedSchedule_ScheduleId", "dbo.ScheduleEntities");
            DropForeignKey("dbo.ResultEntities", "ResultId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "Result_ResultId", "dbo.ResultEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "MemberAtFault_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.IncidentReview_LeagueMember", "MemberRefId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.IncidentReview_LeagueMember", "ReviewRefId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ReviewCommentEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.ReviewCommentEntities", "MemberAtFault_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ReviewCommentEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ReviewCommentEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ReviewCommentEntities", "Author_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "Author_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ResultRowEntities", "ResultId", "dbo.ResultEntities");
            DropForeignKey("dbo.ResultRowEntities", "Member_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ResultEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ResultEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.SessionBaseEntities", "Schedule_ScheduleId", "dbo.ScheduleEntities");
            DropForeignKey("dbo.SessionBaseEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.SessionBaseEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScheduleEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScheduleEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.SeasonEntities", "LastModifiedBy_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.SeasonEntities", "CreatedBy_MemberId", "dbo.LeagueMemberEntities");
            DropIndex("dbo.Scoring_Session", new[] { "SessionRefId" });
            DropIndex("dbo.Scoring_Session", new[] { "ScoringRefId" });
            DropIndex("dbo.MultiScoringMap", new[] { "ScoringChildId" });
            DropIndex("dbo.MultiScoringMap", new[] { "ScoringParentId" });
            DropIndex("dbo.IncidentReview_LeagueMember", new[] { "MemberRefId" });
            DropIndex("dbo.IncidentReview_LeagueMember", new[] { "ReviewRefId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "FastestLapDriver_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "ScoringId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "ResultId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ResultRowId", "ResultId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "MemberAtFault_MemberId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "Author_MemberId" });
            DropIndex("dbo.ReviewCommentEntities", new[] { "ReviewId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "Result_ResultId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "MemberAtFault_MemberId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "Author_MemberId" });
            DropIndex("dbo.ResultRowEntities", new[] { "Member_MemberId" });
            DropIndex("dbo.ResultRowEntities", new[] { "ResultId" });
            DropIndex("dbo.ResultEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ResultEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ResultEntities", new[] { "ResultId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "Schedule_ScheduleId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ScheduleEntities", new[] { "Season_SeasonId" });
            DropIndex("dbo.ScheduleEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ScheduleEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ScoringEntities", new[] { "Season_SeasonId" });
            DropIndex("dbo.ScoringEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.ScoringEntities", new[] { "CreatedBy_MemberId" });
            DropIndex("dbo.ScoringEntities", new[] { "ConnectedSchedule_ScheduleId" });
            DropIndex("dbo.SeasonEntities", new[] { "MainScoring_ScoringId" });
            DropIndex("dbo.SeasonEntities", new[] { "LastModifiedBy_MemberId" });
            DropIndex("dbo.SeasonEntities", new[] { "CreatedBy_MemberId" });
            DropTable("dbo.Scoring_Session");
            DropTable("dbo.MultiScoringMap");
            DropTable("dbo.IncidentReview_LeagueMember");
            DropTable("dbo.ScoredResultEntities");
            DropTable("dbo.ScoredResultRowEntities");
            DropTable("dbo.ReviewCommentEntities");
            DropTable("dbo.IncidentReviewEntities");
            DropTable("dbo.ResultRowEntities");
            DropTable("dbo.ResultEntities");
            DropTable("dbo.SessionBaseEntities");
            DropTable("dbo.ScheduleEntities");
            DropTable("dbo.ScoringEntities");
            DropTable("dbo.SeasonEntities");
            DropTable("dbo.LeagueMemberEntities");
        }
    }
}
