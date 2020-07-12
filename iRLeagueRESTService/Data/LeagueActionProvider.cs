using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using iRLeagueDatabase;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Results;

namespace iRLeagueRESTService.Data
{
    public class LeagueActionProvider : ILeagueActionProvider, IDisposable
    {
        private LeagueDbContext DbContext { get; }

        public LeagueActionProvider(LeagueDbContext context)
        {
            DbContext = context;
        }

        public void Dispose()
        {
            ((IDisposable)DbContext).Dispose();
        }

        public void CalculateScoredResult(long sessionId)
        {
            CalculateScoredResultArray(new long[] { sessionId });
        }

        public void CalculateScoredResultArray(long[] sessionIds)
        {
            DbContext.Configuration.LazyLoadingEnabled = false;
            var sessions = DbContext.Set<SessionBaseEntity>()
                .Include(x => x.Scorings.Select(y => y.ScoredResults))
                .Include(x => x.SessionResult.ScoredResults.Select(y => y.FinalResults.Select(z => z.AddPenalty)))
                .Include(x => x.SessionResult.RawResults.Select(y => y.Member))
                .Where(x => sessionIds.Contains(x.SessionId));

            foreach (var session in sessions)
            {
                var scorings = session.Scorings;

                foreach (var scoring in scorings)
                {
                    scoring.CalculateResults(session, DbContext);
                }
            }

            DbContext.SaveChanges();
            DbContext.Configuration.LazyLoadingEnabled = true;
        }
    }
}