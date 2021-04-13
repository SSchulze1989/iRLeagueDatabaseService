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
                    TeamId = c.Long(),
                })
                .PrimaryKey(t => t.MemberId)
                .ForeignKey("dbo.TeamEntities", t => t.TeamId)
                .Index(t => t.TeamId);

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
                "dbo.SeasonEntities",
                c => new
                {
                    SeasonId = c.Long(nullable: false, identity: true),
                    SeasonName = c.String(),
                    CreatedOn = c.DateTime(),
                    LastModifiedOn = c.DateTime(),
                    Version = c.Int(nullable: false),
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                    MainScoring_ScoringId = c.Long(),
                })
                .PrimaryKey(t => t.SeasonId)
                .ForeignKey("dbo.ScoringEntities", t => t.MainScoring_ScoringId)
                .Index(t => t.MainScoring_ScoringId);

            CreateTable(
                "dbo.ScoringEntities",
                c => new
                {
                    ScoringId = c.Long(nullable: false, identity: true),
                    ScoringKind = c.Int(nullable: false),
                    Name = c.String(),
                    DropWeeks = c.Int(nullable: false),
                    AverageRaceNr = c.Int(nullable: false),
                    MaxResultsPerGroup = c.Int(nullable: false),
                    TakeGroupAverage = c.Boolean(nullable: false),
                    ExtScoringSourceId = c.Long(),
                    TakeResultsFromExtSource = c.Boolean(nullable: false),
                    BasePoints = c.String(),
                    BonusPoints = c.String(),
                    IncPenaltyPoints = c.String(),
                    CreatedOn = c.DateTime(),
                    LastModifiedOn = c.DateTime(),
                    Version = c.Int(nullable: false),
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                    ConnectedSchedule_ScheduleId = c.Long(),
                    Season_SeasonId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.ScoringId)
                .ForeignKey("dbo.ScheduleEntities", t => t.ConnectedSchedule_ScheduleId)
                .ForeignKey("dbo.ScoringEntities", t => t.ExtScoringSourceId)
                .ForeignKey("dbo.SeasonEntities", t => t.Season_SeasonId, cascadeDelete: true)
                .Index(t => t.ExtScoringSourceId)
                .Index(t => t.ConnectedSchedule_ScheduleId)
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
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                    Season_SeasonId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.ScheduleId)
                .ForeignKey("dbo.SeasonEntities", t => t.Season_SeasonId, cascadeDelete: true)
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
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
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
                    Schedule_ScheduleId = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.SessionId)
                .ForeignKey("dbo.ScheduleEntities", t => t.Schedule_ScheduleId, cascadeDelete: true)
                .Index(t => t.Schedule_ScheduleId);

            CreateTable(
                "dbo.IncidentReviewEntities",
                c => new
                {
                    ReviewId = c.Long(nullable: false, identity: true),
                    SessionId = c.Long(nullable: false),
                    AuthorUserId = c.String(),
                    AuthorName = c.String(),
                    IncidentKind = c.String(),
                    FullDescription = c.String(),
                    OnLap = c.Int(nullable: false),
                    Corner = c.Int(nullable: false),
                    TimeStamp = c.Time(nullable: false, precision: 7),
                    CreatedOn = c.DateTime(),
                    LastModifiedOn = c.DateTime(),
                    Version = c.Int(nullable: false),
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.SessionId);

            CreateTable(
                "dbo.AcceptedReviewVoteEntities",
                c => new
                {
                    ReviewVoteId = c.Long(nullable: false, identity: true),
                    ReviewId = c.Long(nullable: false),
                    MemberAtFaultId = c.Long(),
                    Vote = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ReviewVoteId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberAtFaultId)
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReviewId)
                .Index(t => t.MemberAtFaultId);

            CreateTable(
                "dbo.CommentBaseEntities",
                c => new
                {
                    CommentId = c.Long(nullable: false, identity: true),
                    Date = c.DateTime(),
                    AuthorUserId = c.String(),
                    AuthorName = c.String(),
                    Text = c.String(),
                    ReplyToCommentId = c.Long(),
                    CreatedOn = c.DateTime(),
                    LastModifiedOn = c.DateTime(),
                    Version = c.Int(nullable: false),
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                    ReviewId = c.Long(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.CommentBaseEntities", t => t.ReplyToCommentId)
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReplyToCommentId)
                .Index(t => t.ReviewId);

            CreateTable(
                "dbo.CommentReviewVoteEntities",
                c => new
                {
                    ReviewVoteId = c.Long(nullable: false, identity: true),
                    CommentId = c.Long(nullable: false),
                    MemberAtFaultId = c.Long(),
                    Vote = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ReviewVoteId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberAtFaultId)
                .ForeignKey("dbo.CommentBaseEntities", t => t.CommentId, cascadeDelete: true)
                .Index(t => t.CommentId)
                .Index(t => t.MemberAtFaultId);

            CreateTable(
                "dbo.ResultEntities",
                c => new
                {
                    ResultId = c.Long(nullable: false),
                    CreatedOn = c.DateTime(),
                    LastModifiedOn = c.DateTime(),
                    Version = c.Int(nullable: false),
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                })
                .PrimaryKey(t => t.ResultId)
                .ForeignKey("dbo.SessionBaseEntities", t => t.ResultId, cascadeDelete: true)
                .Index(t => t.ResultId);

            CreateTable(
                "dbo.ResultRowEntities",
                c => new
                {
                    ResultRowId = c.Long(nullable: false, identity: true),
                    ResultId = c.Long(nullable: false),
                    StartPosition = c.Int(nullable: false),
                    FinishPosition = c.Int(nullable: false),
                    MemberId = c.Long(nullable: false),
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
                })
                .PrimaryKey(t => t.ResultRowId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberId, cascadeDelete: false)
                .ForeignKey("dbo.ResultEntities", t => t.ResultId, cascadeDelete: true)
                .Index(t => t.ResultId)
                .Index(t => t.MemberId);

            CreateTable(
                "dbo.ScoredResultRowEntities",
                c => new
                {
                    ScoredResultRowId = c.Long(nullable: false, identity: true),
                    ScoredResultId = c.Long(nullable: false),
                    ScoringId = c.Long(nullable: false),
                    ResultRowId = c.Long(nullable: false),
                    RacePoints = c.Int(nullable: false),
                    BonusPoints = c.Int(nullable: false),
                    PenaltyPoints = c.Int(nullable: false),
                    FinalPosition = c.Int(nullable: false),
                    FinalPositionChange = c.Int(nullable: false),
                    TotalPoints = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ScoredResultRowId)
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ScoredResultId, t.ScoringId }, cascadeDelete: false)
                .ForeignKey("dbo.ResultRowEntities", t => t.ResultRowId, cascadeDelete: true)
                .Index(t => new { t.ScoredResultId, t.ScoringId })
                .Index(t => t.ResultRowId);

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

            CreateTable(
                "dbo.ReviewPenaltyEntities",
                c => new
                {
                    ResultRowId = c.Long(nullable: false),
                    ReviewId = c.Long(nullable: false),
                    PenaltyPoints = c.Int(nullable: false),
                    ReviewVoteId = c.Long(nullable: true)
                })
                .PrimaryKey(t => new { t.ResultRowId, t.ReviewId })
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewId, cascadeDelete: false)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.ResultRowId, cascadeDelete: true)
                .ForeignKey("dbo.AcceptedReviewVoteEntities", t=> t.ReviewVoteId)
                .Index(t => t.ResultRowId)
                .Index(t => t.ReviewId)
                .Index(t => t.ReviewVoteId);

            CreateTable(
                "dbo.ScoredResultEntities",
                c => new
                {
                    ResultId = c.Long(nullable: false),
                    ScoringId = c.Long(nullable: false),
                    FastestLap = c.Long(nullable: false),
                    FastestQualyLap = c.Long(nullable: false),
                    FastestAvgLap = c.Long(nullable: false),
                    CreatedOn = c.DateTime(),
                    LastModifiedOn = c.DateTime(),
                    Version = c.Int(nullable: false),
                    CreatedByUserId = c.String(),
                    CreatedByUserName = c.String(),
                    LastModifiedByUserId = c.String(),
                    LastModifiedByUserName = c.String(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                    FastestAvgLapDriver_MemberId = c.Long(),
                    FastestLapDriver_MemberId = c.Long(),
                    FastestQualyLapDriver_MemberId = c.Long(),
                })
                .PrimaryKey(t => new { t.ResultId, t.ScoringId })
                .ForeignKey("dbo.LeagueMemberEntities", t => t.FastestAvgLapDriver_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.FastestLapDriver_MemberId)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.FastestQualyLapDriver_MemberId)
                .ForeignKey("dbo.ResultEntities", t => t.ResultId, cascadeDelete: true)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringId, cascadeDelete: false)
                .Index(t => t.ResultId)
                .Index(t => t.ScoringId)
                .Index(t => t.FastestAvgLapDriver_MemberId)
                .Index(t => t.FastestLapDriver_MemberId)
                .Index(t => t.FastestQualyLapDriver_MemberId);

            CreateTable(
                "dbo.ScoredTeamResultRowEntities",
                c => new
                {
                    ScoredResultRowId = c.Long(nullable: false, identity: true),
                    ScoredResultId = c.Long(nullable: false),
                    ScoringId = c.Long(nullable: false),
                    TeamId = c.Long(nullable: false),
                    Date = c.DateTime(),
                    ClassId = c.Int(nullable: false),
                    CarClass = c.String(),
                    RacePoints = c.Int(nullable: false),
                    BonusPoints = c.Int(nullable: false),
                    PenaltyPoints = c.Int(nullable: false),
                    FinalPosition = c.Int(nullable: false),
                    FinalPositionChange = c.Int(nullable: false),
                    TotalPoints = c.Int(nullable: false),
                    AvgLapTime = c.Long(nullable: false),
                    FastestLapTime = c.Long(nullable: false),
                })
                .PrimaryKey(t => t.ScoredResultRowId)
                .ForeignKey("dbo.TeamEntities", t => t.TeamId, cascadeDelete: false)
                .ForeignKey("dbo.ScoredResultEntities", t => new { t.ScoredResultId, t.ScoringId }, cascadeDelete: true)
                .Index(t => new { t.ScoredResultId, t.ScoringId })
                .Index(t => t.TeamId);

            CreateTable(
                "dbo.ScoringTableEntities",
                c => new
                {
                    ScoringTableId = c.Long(nullable: false, identity: true),
                    ScoringKind = c.Int(nullable: false),
                    Name = c.String(),
                    DropWeeks = c.Int(nullable: false),
                    AverageRaceNr = c.Int(nullable: false),
                    ScoringFactors = c.String(),
                    DropRacesOption = c.Int(nullable: false),
                    ResultsPerRaceCount = c.Int(nullable: false),
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

            CreateTable(
                "dbo.IncidentReview_InvolvedLeagueMember",
                c => new
                {
                    ReviewRefId = c.Long(nullable: false),
                    MemberRefId = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.ReviewRefId, t.MemberRefId })
                .ForeignKey("dbo.IncidentReviewEntities", t => t.ReviewRefId, cascadeDelete: false)
                .ForeignKey("dbo.LeagueMemberEntities", t => t.MemberRefId, cascadeDelete: true)
                .Index(t => t.ReviewRefId)
                .Index(t => t.MemberRefId);

            CreateTable(
                "dbo.ScoredTeamResultRowsGroup",
                c => new
                {
                    ScoredTeamResultRowRefId = c.Long(nullable: false),
                    ScoredResultRowRefId = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.ScoredTeamResultRowRefId, t.ScoredResultRowRefId })
                .ForeignKey("dbo.ScoredTeamResultRowEntities", t => t.ScoredTeamResultRowRefId, cascadeDelete: false)
                .ForeignKey("dbo.ScoredResultRowEntities", t => t.ScoredResultRowRefId, cascadeDelete: true)
                .Index(t => t.ScoredTeamResultRowRefId)
                .Index(t => t.ScoredResultRowRefId);

            CreateTable(
                "dbo.Scoring_Session",
                c => new
                {
                    ScoringRefId = c.Long(nullable: false),
                    SessionRefId = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.ScoringRefId, t.SessionRefId })
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringRefId, cascadeDelete: false)
                .ForeignKey("dbo.SessionBaseEntities", t => t.SessionRefId, cascadeDelete: true)
                .Index(t => t.ScoringRefId)
                .Index(t => t.SessionRefId);

            CreateTable(
                "dbo.ScoringTableMap",
                c => new
                {
                    ScoringTableRefId = c.Long(nullable: false),
                    ScoringRefId = c.Long(nullable: false),
                })
                .PrimaryKey(t => new { t.ScoringTableRefId, t.ScoringRefId })
                .ForeignKey("dbo.ScoringTableEntities", t => t.ScoringTableRefId, cascadeDelete: false)
                .ForeignKey("dbo.ScoringEntities", t => t.ScoringRefId, cascadeDelete: true)
                .Index(t => t.ScoringTableRefId)
                .Index(t => t.ScoringRefId);

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoringTableEntities", "Season_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.ScoringTableMap", "ScoringRefId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringTableMap", "ScoringTableRefId", "dbo.ScoringTableEntities");
            DropForeignKey("dbo.ScheduleEntities", "Season_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.SeasonEntities", "MainScoring_ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.Scoring_Session", "SessionRefId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.Scoring_Session", "ScoringRefId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringEntities", "Season_SeasonId", "dbo.SeasonEntities");
            DropForeignKey("dbo.ScoredResultEntities", "ScoringId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringEntities", "ExtScoringSourceId", "dbo.ScoringEntities");
            DropForeignKey("dbo.ScoringEntities", "ConnectedSchedule_ScheduleId", "dbo.ScheduleEntities");
            DropForeignKey("dbo.ResultEntities", "ResultId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.ScoredResultEntities", "ResultId", "dbo.ResultEntities");
            DropForeignKey("dbo.ResultRowEntities", "ResultId", "dbo.ResultEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", "ResultRowId", "dbo.ResultRowEntities");
            DropForeignKey("dbo.ScoredTeamResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredTeamResultRowEntities", "TeamId", "dbo.TeamEntities");
            DropForeignKey("dbo.ScoredTeamResultRowsGroup", "ScoredResultRowRefId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.ScoredTeamResultRowsGroup", "ScoredTeamResultRowRefId", "dbo.ScoredTeamResultRowEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" }, "dbo.ScoredResultEntities");
            DropForeignKey("dbo.ScoredResultEntities", "FastestQualyLapDriver_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResultEntities", "FastestLapDriver_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ScoredResultEntities", "FastestAvgLapDriver_MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.ReviewPenaltyEntities", "ResultRowId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.ReviewPenaltyEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.AddPenaltyEntities", "ScoredResultRowId", "dbo.ScoredResultRowEntities");
            DropForeignKey("dbo.ResultRowEntities", "MemberId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.SessionBaseEntities", "Schedule_ScheduleId", "dbo.ScheduleEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "SessionId", "dbo.SessionBaseEntities");
            DropForeignKey("dbo.IncidentReview_InvolvedLeagueMember", "MemberRefId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.IncidentReview_InvolvedLeagueMember", "ReviewRefId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.CommentBaseEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.CommentBaseEntities", "ReplyToCommentId", "dbo.CommentBaseEntities");
            DropForeignKey("dbo.CommentReviewVoteEntities", "CommentId", "dbo.CommentBaseEntities");
            DropForeignKey("dbo.CommentReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "ReviewId", "dbo.IncidentReviewEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "MemberAtFaultId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.LeagueMemberEntities", "TeamId", "dbo.TeamEntities");
            DropIndex("dbo.ScoringTableMap", new[] { "ScoringRefId" });
            DropIndex("dbo.ScoringTableMap", new[] { "ScoringTableRefId" });
            DropIndex("dbo.Scoring_Session", new[] { "SessionRefId" });
            DropIndex("dbo.Scoring_Session", new[] { "ScoringRefId" });
            DropIndex("dbo.ScoredTeamResultRowsGroup", new[] { "ScoredResultRowRefId" });
            DropIndex("dbo.ScoredTeamResultRowsGroup", new[] { "ScoredTeamResultRowRefId" });
            DropIndex("dbo.IncidentReview_InvolvedLeagueMember", new[] { "MemberRefId" });
            DropIndex("dbo.IncidentReview_InvolvedLeagueMember", new[] { "ReviewRefId" });
            DropIndex("dbo.ScoringTableEntities", new[] { "Season_SeasonId" });
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "TeamId" });
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "FastestQualyLapDriver_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "FastestLapDriver_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "FastestAvgLapDriver_MemberId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "ScoringId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "ResultId" });
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "ReviewId" });
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "ResultRowId" });
            DropIndex("dbo.AddPenaltyEntities", new[] { "ScoredResultRowId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ResultRowId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "ScoredResultId", "ScoringId" });
            DropIndex("dbo.ResultRowEntities", new[] { "MemberId" });
            DropIndex("dbo.ResultRowEntities", new[] { "ResultId" });
            DropIndex("dbo.ResultEntities", new[] { "ResultId" });
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "MemberAtFaultId" });
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "CommentId" });
            DropIndex("dbo.CommentBaseEntities", new[] { "ReviewId" });
            DropIndex("dbo.CommentBaseEntities", new[] { "ReplyToCommentId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "MemberAtFaultId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "ReviewId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "SessionId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "Schedule_ScheduleId" });
            DropIndex("dbo.ScheduleEntities", new[] { "Season_SeasonId" });
            DropIndex("dbo.ScoringEntities", new[] { "Season_SeasonId" });
            DropIndex("dbo.ScoringEntities", new[] { "ConnectedSchedule_ScheduleId" });
            DropIndex("dbo.ScoringEntities", new[] { "ExtScoringSourceId" });
            DropIndex("dbo.SeasonEntities", new[] { "MainScoring_ScoringId" });
            DropIndex("dbo.LeagueMemberEntities", new[] { "TeamId" });
            DropTable("dbo.ScoringTableMap");
            DropTable("dbo.Scoring_Session");
            DropTable("dbo.ScoredTeamResultRowsGroup");
            DropTable("dbo.IncidentReview_InvolvedLeagueMember");
            DropTable("dbo.ScoringTableEntities");
            DropTable("dbo.ScoredTeamResultRowEntities");
            DropTable("dbo.ScoredResultEntities");
            DropTable("dbo.ReviewPenaltyEntities");
            DropTable("dbo.AddPenaltyEntities");
            DropTable("dbo.ScoredResultRowEntities");
            DropTable("dbo.ResultRowEntities");
            DropTable("dbo.ResultEntities");
            DropTable("dbo.CommentReviewVoteEntities");
            DropTable("dbo.CommentBaseEntities");
            DropTable("dbo.AcceptedReviewVoteEntities");
            DropTable("dbo.IncidentReviewEntities");
            DropTable("dbo.SessionBaseEntities");
            DropTable("dbo.ScheduleEntities");
            DropTable("dbo.ScoringEntities");
            DropTable("dbo.SeasonEntities");
            DropTable("dbo.TeamEntities");
            DropTable("dbo.LeagueMemberEntities");
        }
    }
}
