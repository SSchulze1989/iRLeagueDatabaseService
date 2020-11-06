using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public class DriverStatisticRowEntity
    {
        [Key, ForeignKey(nameof(DriverStatistic)), Column(Order = 1)]
        public long DriverStatisticId { get; set; }
        public virtual DriverStatisticEntity DriverStatistic { get; set; }

        [Key, ForeignKey(nameof(Member)), Column(Order = 2)]
        public long MemberId { get; set; }
        public virtual LeagueMemberEntity Member { get; set; }

        public int StartIRating { get; set; }
        public int EndIRating { get; set; }

        public int StartSRating { get; set; }
        public int EndSRating { get; set; }

        [ForeignKey(nameof(FirstSession))]
        public long FirstSessionId { get; set; }
        public virtual SessionBaseEntity FirstSession { get; set; }

        [ForeignKey(nameof(FirstRace))]
        public long FirstRaceId { get; set; }
        public virtual RaceSessionEntity FirstRace { get; set; }

        [ForeignKey(nameof(LastSession))]
        public long LastSessionId { get; set; }
        public virtual SessionBaseEntity LastSession { get; set; }

        [ForeignKey(nameof(LastRace))]
        public long LastRaceId { get; set; }
        public virtual RaceSessionEntity LastRace { get; set; }

        public int Races { get; set; }
        public int Wins { get; set; }
        public int Poles { get; set; }
        public int Top3 { get; set; }
        public int Top5 { get; set; }
        public int Top10 { get; set; }
        public int Top15 { get; set; }
        public int Top20 { get; set; }
        public int Top25 { get; set; }
        public int RacesInPoints { get; set; }
        public int RacesCompleted { get; set; }
        public int Incidents { get; set; }
        public int PenaltyPoints { get; set; }
        public int FastestLaps { get; set; }
        public int IncidentsUnderInvestigation { get; set; }
        public int IncidentsWithPenalty { get; set; }
        public double DrivenKm { get; set; }
        public double LeadingLaps { get; set; }
        public double LeadingKm { get; set; }
        public double AvgFinishPosition { get; set; }
        public double AvgFinalPosition { get; set; }
        public double AvgStartPosition { get; set; }
        public double AvgPointsPerRace { get; set; }
        public double AvgIncidentsPerRace { get; set; }
        public double AvgIncidentsPerLap { get; set; }
        public double AvgIncidentsPerKm { get; set; }
        public double AvgPenaltyPointsPerRace { get; set; }
        public double AvgPenaltyPointsPerLap { get; set; }
        public double AvgPenaltyPointsPerKm { get; set; }
        public double AvgIRating { get; set; }
        public double AvgSRating { get; set; }
        public int BestFinishPosition { get; set; }
        public int WorstFinishPosition { get; set; }
        public int FirstRaceFinishPosition { get; set; }
        public int LastRaceFinishPosition { get; set; }
        public int BestFinalPosition { get; set; }
        public int WorstFinalPosition { get; set; }
        public int FirstRaceFinalPosition { get; set; }
        public int LastRaceFinalPosition { get; set; }
        public int BestStartPosition { get; set; }
        public int WorstStartPosition { get; set; }
        public int FirstRaceStartPosition { get; set; }
        public int LastRaceStartPosition { get; set; }
    }
}
