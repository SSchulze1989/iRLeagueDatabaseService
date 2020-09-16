using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading.Tasks;

using iRLeagueDatabase;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Results;

namespace iRLeagueRESTService.Data
{
    public class LeagueActionProvider : ILeagueActionProvider, IDisposable
    {
        private LeagueDbContext DbContext { get; }

        public string UserName { get; set; }
        public string UserId { get; set; }

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
                .Include(x => x.SessionResult.ScoredResults.Select(y => y.FinalResults.Select(z => z.ReviewPenalties)))
                //.Include(x => x.SessionResult.ScoredResults.Select(y => ((ScoredTeamResultEntity)y).TeamResults.Select(z => z.ScoredResultRows)))
                .Include(x => x.SessionResult.RawResults.Select(y => y.Member.Team))
                .Include(x => x.Reviews.Select(y => y.AcceptedReviewVotes))
                .Where(x => sessionIds.Contains(x.SessionId));

            foreach (var session in sessions)
            {
                var scorings = session.Scorings;

                foreach (var scoring in scorings)
                {
                    scoring.CalculateResults(session, DbContext);
                }

                foreach (var scoredResult in session.SessionResult.ScoredResults.ToList())
                {
                    if (scoredResult != null && !session.Scorings.Contains(scoredResult.Scoring))
                    {
                        scoredResult.Delete(DbContext);
                        session.SessionResult.ScoredResults.Remove(scoredResult);
                    }
                }
            }

            DbContext.SaveChanges();
            DbContext.Configuration.LazyLoadingEnabled = true;
        }
    }
}