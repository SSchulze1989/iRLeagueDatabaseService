using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Results
{
    [NotMapped]
    public class StandingsEntity : LeagueMappableEntity
    {
        public virtual ScoringTableEntity ScoringTable { get; set; }
        public virtual ScoringEntity Scoring { get; set; }
        public override object MappingId => new long[] { Scoring.ScoringId };
        public virtual List<StandingsRowEntity> StandingsRows { get; set; }
        public virtual LeagueMemberEntity MostWinsDriver { get; set; }
        public virtual LeagueMemberEntity MostPolesDriver { get; set; }
        public virtual LeagueMemberEntity CleanestDriver { get; set; }
        public virtual LeagueMemberEntity MostPenaltiesDriver { get; set; }
        public long SessionId { get; set; }

        public StandingsEntity()
        {
            StandingsRows = new List<StandingsRowEntity>();
        }

        public void Calculate()
        {
            if (StandingsRows != null && StandingsRows.Count >= 0)
            {
                MostWinsDriver = StandingsRows.MaxBy(x => x.Wins).Member;
                MostPolesDriver = StandingsRows.MaxBy(x => x.PolePositions).Member;
                CleanestDriver = StandingsRows.MinBy(x => x.Incidents).Member;
                MostPenaltiesDriver = StandingsRows.MaxBy(x => x.PenaltyPoints).Member;
            }
        }

        //public StandingsEntity CalculateChanges(StandingsEntity previous)
        //{
        //    if (this. Scoring.ScoringId != previous.Scoring.ScoringId)
        //    {
        //        throw new InvalidOperationException("Scoring Entities of calculated Standings do not match. Can not calculate changes between different Scoring tables!");
        //    }

        //    StandingsEntity result = new StandingsEntity()
        //    {
        //        Scoring = this.Scoring,
        //        MostWinsDriver = this.MostWinsDriver,
        //        MostPolesDriver = this.MostPolesDriver,
        //        CleanestDriver = this.CleanestDriver,
        //        MostPenaltiesDriver = this.MostPenaltiesDriver,
        //        StandingsRows = this.StandingsRows.Diff(previous.StandingsRows).ToList()
        //    };

        //    return result;
        //}

        public override void Delete(LeagueDbContext dbContext)
        {
            throw new InvalidOperationException("Could not delete " + GetType().Name + ". Entity is not attached to database.");
        }

        public override long GetLeagueId()
        {
            return ScoringTable.GetLeagueId();
        }
    }
}
