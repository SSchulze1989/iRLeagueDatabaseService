namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeagueIdColToAll : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoringEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ScheduleEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.SessionBaseEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.IncidentReviewEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.AcceptedReviewVoteEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredTeamResultRowEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultRowEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.AddPenaltyEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ResultRowEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ResultEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.IRSimSessionDetailsEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoredResultEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ReviewPenaltyEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.CommentReviewVoteEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ResultsFilterOptionEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.ScoringTableEntities", "LeagueId", c => c.Long(nullable: false));
            AddColumn("dbo.DriverStatisticRowEntities", "LeagueId", c => c.Long(nullable: false));
            Sql("UPDATE dbo.ScoredTeamResultRowEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ScoredResultRowEntities SET LeagueId = 1");
            Sql("UPDATE dbo.AddPenaltyEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ResultRowEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ResultEntities SET LeagueId = 1");
            Sql("UPDATE dbo.IRSimSessionDetailsEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ScoredResultEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ScoringEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ScheduleEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ScoringTableEntities SET LeagueId = 1");
            Sql("UPDATE dbo.DriverStatisticRowEntities SET LeagueId = 1");
            Sql("UPDATE dbo.SessionBaseEntities SET LeagueId = 1");
            Sql("UPDATE dbo.IncidentReviewEntities SET LeagueId = 1");
            Sql("UPDATE dbo.AcceptedReviewVoteEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ReviewPenaltyEntities SET LeagueId = 1");
            Sql("UPDATE dbo.CommentReviewVoteEntities SET LeagueId = 1");
            Sql("UPDATE dbo.ResultsFilterOptionEntities SET LeagueId = 1");
            CreateIndex("dbo.ScoredTeamResultRowEntities", "LeagueId");
            CreateIndex("dbo.ScoredResultRowEntities", "LeagueId");
            CreateIndex("dbo.AddPenaltyEntities", "LeagueId");
            CreateIndex("dbo.ResultRowEntities", "LeagueId");
            CreateIndex("dbo.ResultEntities", "LeagueId");
            CreateIndex("dbo.IRSimSessionDetailsEntities", "LeagueId");
            CreateIndex("dbo.ScoredResultEntities", "LeagueId");
            CreateIndex("dbo.ScoringEntities", "LeagueId");
            CreateIndex("dbo.ScheduleEntities", "LeagueId");
            CreateIndex("dbo.ScoringTableEntities", "LeagueId");
            CreateIndex("dbo.DriverStatisticRowEntities", "LeagueId");
            CreateIndex("dbo.SessionBaseEntities", "LeagueId");
            CreateIndex("dbo.IncidentReviewEntities", "LeagueId");
            CreateIndex("dbo.AcceptedReviewVoteEntities", "LeagueId");
            CreateIndex("dbo.ReviewPenaltyEntities", "LeagueId");
            CreateIndex("dbo.CommentReviewVoteEntities", "LeagueId");
            CreateIndex("dbo.ResultsFilterOptionEntities", "LeagueId");
            AddForeignKey("dbo.ScoredTeamResultRowEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.AddPenaltyEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.ScoredResultRowEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.ResultRowEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.IRSimSessionDetailsEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.ResultEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.ScoredResultEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.ScheduleEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.ScoringTableEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.SessionBaseEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.AcceptedReviewVoteEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.ReviewPenaltyEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.CommentReviewVoteEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.IncidentReviewEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.DriverStatisticRowEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
            AddForeignKey("dbo.ScoringEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: true);
            AddForeignKey("dbo.ResultsFilterOptionEntities", "LeagueId", "dbo.LeagueEntities", "LeagueId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResultsFilterOptionEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ScoringEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.DriverStatisticRowEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.IncidentReviewEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.CommentReviewVoteEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ReviewPenaltyEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.AcceptedReviewVoteEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.SessionBaseEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ScoringTableEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ScheduleEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ScoredResultEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ResultEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.IRSimSessionDetailsEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ResultRowEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ScoredResultRowEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.AddPenaltyEntities", "LeagueId", "dbo.LeagueEntities");
            DropForeignKey("dbo.ScoredTeamResultRowEntities", "LeagueId", "dbo.LeagueEntities");
            DropIndex("dbo.ResultsFilterOptionEntities", new[] { "LeagueId" });
            DropIndex("dbo.CommentReviewVoteEntities", new[] { "LeagueId" });
            DropIndex("dbo.ReviewPenaltyEntities", new[] { "LeagueId" });
            DropIndex("dbo.AcceptedReviewVoteEntities", new[] { "LeagueId" });
            DropIndex("dbo.IncidentReviewEntities", new[] { "LeagueId" });
            DropIndex("dbo.SessionBaseEntities", new[] { "LeagueId" });
            DropIndex("dbo.DriverStatisticRowEntities", new[] { "LeagueId" });
            DropIndex("dbo.ScoringTableEntities", new[] { "LeagueId" });
            DropIndex("dbo.ScheduleEntities", new[] { "LeagueId" });
            DropIndex("dbo.ScoringEntities", new[] { "LeagueId" });
            DropIndex("dbo.ScoredResultEntities", new[] { "LeagueId" });
            DropIndex("dbo.IRSimSessionDetailsEntities", new[] { "LeagueId" });
            DropIndex("dbo.ResultEntities", new[] { "LeagueId" });
            DropIndex("dbo.ResultRowEntities", new[] { "LeagueId" });
            DropIndex("dbo.AddPenaltyEntities", new[] { "LeagueId" });
            DropIndex("dbo.ScoredResultRowEntities", new[] { "LeagueId" });
            DropIndex("dbo.ScoredTeamResultRowEntities", new[] { "LeagueId" });
            DropColumn("dbo.DriverStatisticRowEntities", "LeagueId");
            DropColumn("dbo.ScoringTableEntities", "LeagueId");
            DropColumn("dbo.ResultsFilterOptionEntities", "LeagueId");
            DropColumn("dbo.CommentReviewVoteEntities", "LeagueId");
            DropColumn("dbo.ReviewPenaltyEntities", "LeagueId");
            DropColumn("dbo.ScoredResultEntities", "LeagueId");
            DropColumn("dbo.IRSimSessionDetailsEntities", "LeagueId");
            DropColumn("dbo.ResultEntities", "LeagueId");
            DropColumn("dbo.ResultRowEntities", "LeagueId");
            DropColumn("dbo.AddPenaltyEntities", "LeagueId");
            DropColumn("dbo.ScoredResultRowEntities", "LeagueId");
            DropColumn("dbo.ScoredTeamResultRowEntities", "LeagueId");
            DropColumn("dbo.AcceptedReviewVoteEntities", "LeagueId");
            DropColumn("dbo.IncidentReviewEntities", "LeagueId");
            DropColumn("dbo.SessionBaseEntities", "LeagueId");
            DropColumn("dbo.ScheduleEntities", "LeagueId");
            DropColumn("dbo.ScoringEntities", "LeagueId");
        }
    }
}
