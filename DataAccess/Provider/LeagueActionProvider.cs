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
using System.Runtime.CompilerServices;

namespace iRLeagueDatabase.DataAccess.Provider
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
            /// Get sessions to actualize the sessionlist in case some sessions have been added to the schedule in the meantime
            IQueryable<ScoringEntity> allScorings = DbContext.Set<ScoringEntity>()
                .Include(x => x.Season)
                .Include(x => x.Sessions)
                .Include(x => x.ConnectedSchedule)
                .Include(x => x.ScoredResults);
            allScorings.ToList().ForEach(x => x.GetAllSessions());

            var sessions = DbContext.Set<SessionBaseEntity>()
                //.Include(x => x.Scorings.Select(y => y.ScoredResults))
                //.Include(x => x.SessionResult.ScoredResults.Select(y => y.FinalResults.Select(z => z.AddPenalty)))
                //.Include(x => x.SessionResult.ScoredResults.Select(y => y.FinalResults.Select(z => z.ReviewPenalties)))
                //.Include(x => x.SessionResult.ScoredResults.Select(y => ((ScoredTeamResultEntity)y).TeamResults.Select(z => z.ScoredResultRows)))
                //.Include(x => x.SessionResult.RawResults.Select(y => y.Member.Team))
                .Include(x => x.Reviews.Select(y => y.AcceptedReviewVotes.Select(z => z.CustomVoteCat)))
                .Include(x => x.SubSessions)
                .Include(x => x.Schedule)
                .Where(x => sessionIds.Contains(x.SessionId));

            var allSessionIds = sessionIds.AsEnumerable();
            allSessionIds = allSessionIds.Concat(sessions.SelectMany(x => x.SubSessions.Select(y => y.SessionId)));

            DbContext.Set<ResultEntity>()
                .Where(x => allSessionIds.Contains(x.ResultId))
                .Include(x => x.RawResults.Select(y => y.Member.Team))
                .Load();

            DbContext.Set<ScoredResultEntity>()
                .Where(x => allSessionIds.Contains(x.ResultId))
                .Include(x => x.FinalResults.Select(y => y.ResultRow.Member.Team))
                .Include(x => x.FinalResults.Select(y => y.AddPenalty))
                .Include(x => x.FinalResults.Select(y => y.ReviewPenalties))
                .Include(x => x.HardChargers)
                .Include(x => x.CleanestDrivers)
                .Include(x => x.Result)
                .Load();

            DbContext.Set<ScoredTeamResultRowEntity>()
                .Where(x => allSessionIds.Contains(x.ScoredResultId))
                .Include(x => x.ScoredResultRows)
                .Load();

            DbContext.ChangeTracker.DetectChanges();

            foreach (var session in sessions)
            {
                IEnumerable<ScoringEntity> scorings = session.Scorings;
                scorings = scorings.Concat(session.SubSessions.SelectMany(x => x.Scorings)).Where(x => x != null);

                // reorder scorings to get subsession scorings recalculated before accumulated scorings
                scorings = scorings
                    .OrderBy(x => x.AccumulateResultsOption == iRLeagueManager.Enums.AccumulateResultsOption.None)
                    .OrderBy(x => x.ExtScoringSourceId != null)
                    .OrderBy(x => x.ParentScoringId == null);

                foreach (var scoring in scorings)
                {
                    scoring.CalculateResults(session, DbContext);
                }

                foreach (var scoredResult in session.SessionResult.ScoredResults.ToList())
                {
                    if (scoredResult != null && session.Scorings.Contains(scoredResult.Scoring) == false)
                    {
                        scoredResult.Delete(DbContext);
                        session.SessionResult.ScoredResults.Remove(scoredResult);
                    }
                }
                session.SessionResult.RequiresRecalculation = false;
            }

            DbContext.SaveChanges();
            DbContext.Configuration.LazyLoadingEnabled = true;
        }
    }
}