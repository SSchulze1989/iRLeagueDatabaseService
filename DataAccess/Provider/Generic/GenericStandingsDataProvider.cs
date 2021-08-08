using iRLeagueDatabase;
using iRLeagueDatabase.DataAccess.Mapper;
using iRLeagueDatabase.DataAccess.Provider;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider.Generic
{
    public class GenericStandingsDataProvider : GenericDataProviderBase<StandingsDataDTO, long[]>, IDataProvider<StandingsDataDTO, long[]>
    {
        public GenericStandingsDataProvider(IProviderContext<LeagueDbContext> context) : base(context)
        {
        }

        public override StandingsDataDTO GetData(long[] key)
        {
            return GetData(new long[][] { key }).FirstOrDefault();
        }

        public override IEnumerable<StandingsDataDTO> GetData(IEnumerable<long[]> keys)
        {
            return GetStandings(keys?.ToArray());
        }

        public StandingsDataDTO[] GetStandings(long[] scoringIds)
        {
            return GetStandings(scoringIds?.Select(x => new long[] { x }).ToArray());
        }

        public StandingsDataDTO[] GetStandings(long[][] requestIds)
        {
            var mapper = new DTOMapper(DbContext);

            DbContext.Configuration.LazyLoadingEnabled = false;
            List<StandingsDataDTO> responseItems = new List<StandingsDataDTO>();
            if (requestIds == null)
            {
                return responseItems.ToArray();
            }

            List<long> loadedScoringEntityIds = new List<long>();
            foreach (var requestId in requestIds)
            {
                if (requestId == null)
                {
                    continue;
                }

                var scoringTableId = requestId[0];
                var sessionId = requestId.Count() > 1 ? requestId[1] : 0;
                ScoringTableEntity scoringTable = null;

                try
                {
                    scoringTable = DbContext.Set<ScoringTableEntity>()
                        .Where(x => x.ScoringTableId == scoringTableId)
                        .Include(x => x.Scorings.Select(y => y.ExtScoringSource))
                        //.Include(x => x.Scorings.Select(y => y.Sessions.Select(z => z.SessionResult)))
                        //.Include(x => x.Scorings.Select(y => y.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                        //.Include(x => x.Scorings.Select(y => y.ExtScoringSource.ScoredResults.Select(z => z.FinalResults.Select(q => q.ResultRow.Member))))
                        .FirstOrDefault();

                    if (CheckLeague(DbContext.CurrentLeagueId, scoringTable) == false)
                    {
                        return null;
                    }

                    var loadScoringEntityIds = DbContext.Set<ScoringEntity>().Local.Select(y => y.ScoringId).Except(loadedScoringEntityIds);

                    if (loadScoringEntityIds.Count() > 0)
                    {
                        var loadScorings = DbContext.Set<ScoringEntity>()
                            .Where(x => loadScoringEntityIds.Contains(x.ScoringId))
                            .Include(x => x.Sessions.Select(y => y.SessionResult))
                            .ToList();
                        var loadScoredResults = DbContext.Set<ScoredResultEntity>()
                            .Where(x => loadScoringEntityIds.Contains(x.ScoringId))
                            .Include(x => x.FinalResults.Select(y => y.ResultRow.Member))
                            .Include(x => x.FinalResults.Select(y => y.Team))
                            .ToList();
                    }

                    DbContext.ChangeTracker.DetectChanges();
                }
                catch (Exception e)
                {
                    throw new Exception("Error while getting data from Database.", e);
                }

                try
                {
                    if (scoringTable != null)
                    {
                        StandingsEntity standings;
                        if (sessionId == 0)
                            standings = scoringTable.GetSeasonStandings(DbContext);
                        else
                        {
                            var scoringSession = scoringTable.GetAllSessions().SingleOrDefault(x => x.SessionId == sessionId);
                            if (scoringSession == null)
                            {
                                var session = DbContext.Set<SessionBaseEntity>().Find(sessionId);
                                scoringSession = scoringTable.GetAllSessions().LastOrDefault(x => x.Date <= session?.Date);
                            }
                            standings = scoringTable.GetSeasonStandings(scoringSession, DbContext);
                        }
                        var standingsDTO = mapper.MapTo<StandingsDataDTO>(standings);
                        responseItems.Add(standingsDTO);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error while mapping data.", e);
                }
            }
            DbContext.Configuration.LazyLoadingEnabled = true;

            return responseItems.ToArray();
        }
    }
}
