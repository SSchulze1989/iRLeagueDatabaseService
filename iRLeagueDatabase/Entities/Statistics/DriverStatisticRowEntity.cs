// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
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
    public class DriverStatisticRowEntity : MappableEntity
    {
        [Key, ForeignKey(nameof(StatisticSet)), Column(Order = 1)]
        public long StatisticSetId { get; set; }
        public virtual StatisticSetEntity StatisticSet { get; set; }

        [Key, ForeignKey(nameof(Member)), Column(Order = 2)]
        public long MemberId { get; set; }
        public virtual LeagueMemberEntity Member { get; set; }

        public int StartIRating { get; set; }
        public int EndIRating { get; set; }

        public double StartSRating { get; set; }
        public double EndSRating { get; set; }

        [ForeignKey(nameof(FirstSession))]
        public long? FirstSessionId { get; set; }
        public virtual SessionBaseEntity FirstSession { get; set; }
        public DateTime? FirstSessionDate { get; set; }

        [ForeignKey(nameof(FirstRace))]
        public long? FirstRaceId { get; set; }
        public virtual RaceSessionEntity FirstRace { get; set; }
        public DateTime? FirstRaceDate { get; set; }

        [ForeignKey(nameof(FirstResult)), Column(Order = 3)]
        public long? FirstResultRowId { get; set; }
        public virtual ScoredResultRowEntity FirstResult { get; set; }

        [ForeignKey(nameof(LastSession))]
        public long? LastSessionId { get; set; }
        public virtual SessionBaseEntity LastSession { get; set; }
        public DateTime? LastSessionDate { get; set; }

        [ForeignKey(nameof(LastRace))]
        public long? LastRaceId { get; set; }
        public virtual RaceSessionEntity LastRace { get; set; }
        public DateTime? LastRaceDate { get; set; }

        [ForeignKey(nameof(LastResult)), Column(Order = 4)]
        public long? LastResultRowId { get; set; }
        public virtual ScoredResultRowEntity LastResult { get; set; }

        public double RacePoints { get; set; }
        public double TotalPoints { get; set; }
        public double BonusPoints { get; set; }
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
        public double Incidents { get; set; }
        public double PenaltyPoints { get; set; }
        public int FastestLaps { get; set; }
        public int IncidentsUnderInvestigation { get; set; }
        public int IncidentsWithPenalty { get; set; }
        public double LeadingLaps { get; set; }
        public double CompletedLaps { get; set; }
        public int CurrentSeasonPosition { get; set; }
        public double DrivenKm { get; set; }
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
        public double BestFinishPosition { get; set; }
        public double WorstFinishPosition { get; set; }
        public double FirstRaceFinishPosition { get; set; }
        public double LastRaceFinishPosition { get; set; }
        public int BestFinalPosition { get; set; }
        public int WorstFinalPosition { get; set; }
        public int FirstRaceFinalPosition { get; set; }
        public int LastRaceFinalPosition { get; set; }
        public double BestStartPosition { get; set; }
        public double WorstStartPosition { get; set; }
        public double FirstRaceStartPosition { get; set; }
        public double LastRaceStartPosition { get; set; }
        public int Titles { get; set; }
        public int HardChargerAwards { get; set; }
        public int CleanestDriverAwards { get; set; }

        public override object MappingId => new { StatisticSetId, MemberId };

        public void ResetStatistic()
        {
            RacePoints = default;
            TotalPoints = default;
            BonusPoints = default;
            Races = default;
            Wins = default;
            Poles = default;
            Top3 = default;
            Top5 = default;
            Top10 = default;
            Top15 = default;
            Top20 = default;
            Top25 = default;
            RacesInPoints = default;
            RacesCompleted = default;
            Incidents = default;
            PenaltyPoints = default;
            FastestLaps = default;
            IncidentsUnderInvestigation = default;
            IncidentsWithPenalty = default;
            LeadingLaps = default;
            CompletedLaps = default;
            CurrentSeasonPosition = default;
            DrivenKm = default;
            LeadingKm = default;
            AvgFinishPosition = default;
            AvgFinalPosition = default;
            AvgStartPosition = default;
            AvgPointsPerRace = default;
            AvgIncidentsPerRace = default;
            AvgIncidentsPerLap = default;
            AvgIncidentsPerKm = default;
            AvgPenaltyPointsPerRace = default;
            AvgPenaltyPointsPerLap = default;
            AvgPenaltyPointsPerKm = default;
            AvgIRating = default;
            AvgSRating = default;
            BestFinishPosition = default;
            WorstFinishPosition = default;
            FirstRaceFinishPosition = default;
            LastRaceFinishPosition = default;
            BestFinalPosition = default;
            WorstFinalPosition = default;
            FirstRaceFinalPosition = default;
            LastRaceFinalPosition = default;
            BestStartPosition = default;
            WorstStartPosition = default;
            FirstRaceStartPosition = default;
            LastRaceStartPosition = default;
            StartIRating = default;
            EndIRating = default;
            StartSRating = default;
            EndSRating = default;
            FirstRace = default;
            FirstRaceDate = default;
            FirstSession = default;
            FirstSessionDate = default;
            LastRace = default;
            LastRaceDate = default;
            LastSession = default;
            LastSessionDate = default;
            Titles = default;
            HardChargerAwards = default;
            CleanestDriverAwards = default;
        }
    }
}
