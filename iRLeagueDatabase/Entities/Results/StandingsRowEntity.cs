using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Members;
using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Results
{
    public class StandingsRowEntity : MappableEntity 
    {   
        public int Position { get; set; }
        public int LastPosition { get; set; }
        //[ForeignKey(nameof(Member))]
        //public int MemberId { get; set; }
        public virtual LeagueMemberEntity Member { get; set; }
        public int ClassId { get; set; }
        public string CarClass { get; set; }
        public int RacePoints { get; set; }
        public int RacePointsChange { get; set; }
        public int PenaltyPoints { get; set; }
        public int PenaltyPointsChange { get; set; }
        public int TotalPoints { get; set; }
        public int TotalPointsChange { get; set; }
        public int Races { get; set; }
        public int RacesCounted { get; set; }
        public int DroppedResults { get; set; }
        public int CompletedLaps { get; set; }
        public int CompletedLapsChange { get; set; }
        public int LeadLaps { get; set; }
        public int LeadLapsChange { get; set; }
        public int FastestLaps { get; set; }
        public int PolePositions { get; set; }
        public int Wins { get; set; }
        public int Top3 { get; set; }
        public int Top5 { get; set; }
        public int Top10 { get; set; }
        public int Incidents { get; set; }
        public int IncidentsChange { get; set; }
        public int PositionChange { get; set; }

        public StandingsRowEntity()
        {
            CarClass = "";
            ClassId = 0;
            CompletedLaps = 0;
            CompletedLapsChange = 0;
            DroppedResults = 0;
            FastestLaps = 0;
            Incidents = 0;
            IncidentsChange = 0;
            LastPosition = 0;
            LeadLaps = 0;
            LeadLapsChange = 0;
            PenaltyPoints = 0;
            PenaltyPointsChange = 0;
            PolePositions = 0;
            Position = 0;
            PositionChange = 0;
            RacePoints = 0;
            RacePointsChange = 0;
            Races = 0;
            Top10 = 0;
            Top3 = 0;
            Top5 = 0;
            TotalPoints = 0;
            TotalPointsChange = 0;
            Wins = 0;
        }
    }
}
