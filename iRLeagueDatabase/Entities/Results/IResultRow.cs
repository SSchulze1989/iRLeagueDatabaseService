using iRLeagueDatabase.Entities.Members;
using iRLeagueManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Results
{
    public interface IResultRow
    {
        SimSessionTypeEnum SimSessionType { get; }

        DateTime? Date { get; }

        int StartPosition { get; }

        int FinishPosition { get; }

        long MemberId { get; }
        LeagueMemberEntity Member { get; }

        int OldIRating { get; }
        int NewIRating { get; }
        int SeasonStartIRating { get; }
        string License { get; }
        double OldSafetyRating { get; }
        double NewSafetyRating { get; }
        int OldCpi { get; }
        int NewCpi { get; }
        int ClubId { get; }
        string ClubName { get; }
        int CarNumber { get; }
        int ClassId { get; }
        string Car { get; }
        int CarId { get; }
        string CarClass { get; }
        int CompletedLaps { get; }
        double CompletedPct { get; }
        int LeadLaps { get; }
        int FastLapNr { get; }
        int Incidents { get; }
        RaceStatusEnum Status { get; }
        DateTime? QualifyingTimeAt { get; }
        long QualifyingTime { get; }
        long Interval { get; }
        long AvgLapTime { get; }
        long FastestLapTime { get; }
        int PositionChange { get; }
        int Division { get; }
        int OldLicenseLevel { get; }
        int NewLicenseLevel { get; }
        int NumPitStops { get; }
        string PittedLaps { get; }
        int NumOfftrackLaps { get; }
        string OfftrackLaps { get; }
        int NumContactLaps { get; }
        string ContactLaps { get; }
        int RacePoints { get; }
        int BonusPoints { get; }
        int PenaltyPoints { get; }
        int TotalPoints { get; }
    }
}
