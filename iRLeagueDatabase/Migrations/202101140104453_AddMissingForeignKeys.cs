namespace iRLeagueDatabase.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissingForeignKeys : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ScoringEntities", name: "ConnectedSchedule_ScheduleId", newName: "ConnectedScheduleId");
            RenameColumn(table: "dbo.ScoringEntities", name: "Season_SeasonId", newName: "SeasonId");
            RenameColumn(table: "dbo.SessionBaseEntities", name: "Schedule_ScheduleId", newName: "ScheduleId");
            RenameColumn(table: "dbo.ScoringTableEntities", name: "Season_SeasonId", newName: "SeasonId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_Season_SeasonId", newName: "IX_SeasonId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_ConnectedSchedule_ScheduleId", newName: "IX_ConnectedScheduleId");
            RenameIndex(table: "dbo.ScoringTableEntities", name: "IX_Season_SeasonId", newName: "IX_SeasonId");
            RenameIndex(table: "dbo.SessionBaseEntities", name: "IX_Schedule_ScheduleId", newName: "IX_ScheduleId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.SessionBaseEntities", name: "IX_ScheduleId", newName: "IX_Schedule_ScheduleId");
            RenameIndex(table: "dbo.ScoringTableEntities", name: "IX_SeasonId", newName: "IX_Season_SeasonId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_ConnectedScheduleId", newName: "IX_ConnectedSchedule_ScheduleId");
            RenameIndex(table: "dbo.ScoringEntities", name: "IX_SeasonId", newName: "IX_Season_SeasonId");
            RenameColumn(table: "dbo.ScoringTableEntities", name: "SeasonId", newName: "Season_SeasonId");
            RenameColumn(table: "dbo.SessionBaseEntities", name: "ScheduleId", newName: "Schedule_ScheduleId");
            RenameColumn(table: "dbo.ScoringEntities", name: "SeasonId", newName: "Season_SeasonId");
            RenameColumn(table: "dbo.ScoringEntities", name: "ConnectedScheduleId", newName: "ConnectedSchedule_ScheduleId");
        }
    }
}
