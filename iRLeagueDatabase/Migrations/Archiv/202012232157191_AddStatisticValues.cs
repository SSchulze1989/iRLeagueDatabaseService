namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStatisticValues : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SeasonStatisticSet_Scoring", "SeasonStatisticSetRefId", "dbo.StatisticSetEntities");
            DropForeignKey("dbo.SeasonStatisticSet_Scoring", "ScoringRefId", "dbo.ScoringEntities");
            DropIndex("dbo.SeasonStatisticSet_Scoring", new[] { "SeasonStatisticSetRefId" });
            DropIndex("dbo.SeasonStatisticSet_Scoring", new[] { "ScoringRefId" });
            AddColumn("dbo.StatisticSetEntities", "CurrentChampId", c => c.Long());
            AddColumn("dbo.StatisticSetEntities", "ScoringTableId", c => c.Long());
            AddColumn("dbo.StatisticSetEntities", "FinishedRaces", c => c.Int(nullable: false));
            AddColumn("dbo.StatisticSetEntities", "IsSeasonFinished", c => c.Boolean(nullable: false));
            AddColumn("dbo.DriverStatisticRowEntities", "CurrentSeasonPosition", c => c.Int(nullable: false));
            AddColumn("dbo.SeasonEntities", "Finished", c => c.Boolean(nullable: false));
            CreateIndex("dbo.StatisticSetEntities", "CurrentChampId");
            CreateIndex("dbo.StatisticSetEntities", "ScoringTableId");
            AddForeignKey("dbo.StatisticSetEntities", "ScoringTableId", "dbo.ScoringTableEntities", "ScoringTableId");
            AddForeignKey("dbo.StatisticSetEntities", "CurrentChampId", "dbo.LeagueMemberEntities", "MemberId");
            DropTable("dbo.SeasonStatisticSet_Scoring");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SeasonStatisticSet_Scoring",
                c => new
                    {
                        SeasonStatisticSetRefId = c.Long(nullable: false),
                        ScoringRefId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SeasonStatisticSetRefId, t.ScoringRefId });
            
            DropForeignKey("dbo.StatisticSetEntities", "CurrentChampId", "dbo.LeagueMemberEntities");
            DropForeignKey("dbo.StatisticSetEntities", "ScoringTableId", "dbo.ScoringTableEntities");
            DropIndex("dbo.StatisticSetEntities", new[] { "ScoringTableId" });
            DropIndex("dbo.StatisticSetEntities", new[] { "CurrentChampId" });
            DropColumn("dbo.SeasonEntities", "Finished");
            DropColumn("dbo.DriverStatisticRowEntities", "CurrentSeasonPosition");
            DropColumn("dbo.StatisticSetEntities", "IsSeasonFinished");
            DropColumn("dbo.StatisticSetEntities", "FinishedRaces");
            DropColumn("dbo.StatisticSetEntities", "ScoringTableId");
            DropColumn("dbo.StatisticSetEntities", "CurrentChampId");
            CreateIndex("dbo.SeasonStatisticSet_Scoring", "ScoringRefId");
            CreateIndex("dbo.SeasonStatisticSet_Scoring", "SeasonStatisticSetRefId");
            AddForeignKey("dbo.SeasonStatisticSet_Scoring", "ScoringRefId", "dbo.ScoringEntities", "ScoringId", cascadeDelete: false);
            AddForeignKey("dbo.SeasonStatisticSet_Scoring", "SeasonStatisticSetRefId", "dbo.StatisticSetEntities", "Id", cascadeDelete: true);
        }
    }
}
