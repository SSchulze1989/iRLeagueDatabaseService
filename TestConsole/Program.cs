using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel;
using System.Runtime.CompilerServices;


using iRLeagueDatabase;
//using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Sessions;
//using TestConsole.LeagueDBServiceRef;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.Entities.Members;
using System.Net.Http;
using System.Security.Principal;
using iRLeagueDatabase.Extensions;
using iRLeagueDatabase.Entities.Statistics;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.Mapper;
using System.Diagnostics;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test stored procedure
            using (var dbContext = new LeagueDbContext("SkippyCup_leagueDb"))
            {
                dbContext.Database.Initialize(false);
            }

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            for (int i = 0; i < 10; i++)
            {
                using (var dbContext = new LeagueDbContext("SkippyCup_leagueDb"))
                {
                    dbContext.Configuration.AutoDetectChangesEnabled = false;
                    dbContext.Configuration.LazyLoadingEnabled = false;
                    long[] ids = { 22, 23, 24, 25, 26, 27, 28, 29 };
                    long[] scoringIds = { 18 };
                    //stopWatch.Start();
                    EagerLoadResult(dbContext, ids, scoringIds);
                    //stopWatch.Stop();
                    Console.WriteLine($"Eager load: {stopWatch.ElapsedMilliseconds}");

                    //stopWatch.Reset();
                    //stopWatch.Start();
                    List<ScoredResultDataDTO> results = new List<ScoredResultDataDTO>();
                    var mapper = new DTOMapper(dbContext);
                    foreach (var id in ids)
                    {
                        var resultEntity = dbContext.Set<ScoredResultEntity>().Find(id, 18);
                        results.Add(mapper.MapToScoredResultDataDTO(resultEntity));
                    }
                    //stopWatch.Stop();
                }
            }
            stopWatch.Stop();

            Console.WriteLine($"Exection time: {stopWatch.ElapsedMilliseconds}");
            Console.ReadLine();
        }

        private static void EagerLoadResult(LeagueDbContext dbContext, long[] resultIds, long[] scoringIds)
        {
            dbContext.Database.Initialize(false);

            var cmd = dbContext.Database.Connection.CreateCommand();
            cmd.CommandText = "dbo.GetScoredResults @resultIds, @scoringIds";
            var param = cmd.CreateParameter();
            param.ParameterName = "resultIds";
            param.Value = string.Join(",", resultIds);
            param.DbType = System.Data.DbType.String;
            cmd.Parameters.Add(param);
            var param2 = cmd.CreateParameter();
            param2.ParameterName = "scoringIds";
            param2.Value = string.Join(",", scoringIds);
            param2.DbType = System.Data.DbType.String;
            cmd.Parameters.Add(param2);

            dbContext.Database.Connection.Open();
            try
            {
                var reader = cmd.ExecuteReader();

                var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;

                var scoredResults = objContext
                    .Translate<ScoredResultEntity>(reader, "ScoredResultEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var scoredTeamResults = objContext
                    .Translate<ScoredTeamResultEntity>(reader, "ScoredResultEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var scoredResultRows = objContext
                    .Translate<ScoredResultRowEntity>(reader, "ScoredResultRowEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var scorings = objContext
                    .Translate<ScoringEntity>(reader, "ScoringEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var results = objContext
                    .Translate<ResultEntity>(reader, "ResultEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var resulRows = objContext
                    .Translate<ResultRowEntity>(reader, "ResultRowEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var reviewPenalties = objContext
                    .Translate<ReviewPenaltyEntity>(reader, "ReviewPenaltyEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var acceptedReviewVotes = objContext
                    .Translate<AcceptedReviewVoteEntity>(reader, "AcceptedReviewVoteEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var addPenalties = objContext
                    .Translate<AddPenaltyEntity>(reader, "AddPenaltyEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var reviews = objContext
                    .Translate<IncidentReviewEntity>(reader, "IncidentReviewEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var sessions = objContext
                    .Translate<SessionBaseEntity>(reader, "SessionBaseEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var raceSessions = objContext
                    .Translate<RaceSessionEntity>(reader, "SessionBaseEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var members = objContext
                    .Translate<LeagueMemberEntity>(reader, nameof(LeagueDbContext.Members), System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
                reader.NextResult();
                var teams = objContext
                    .Translate<TeamEntity>(reader, "TeamEntities", System.Data.Entity.Core.Objects.MergeOption.AppendOnly).ToList();
            }
            finally
            {
                dbContext.Database.Connection.Close();
            }
        }

        static void NotifyDirect(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName + " changed. (direct)");
        }

        static void NotifyContainer(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName + " changed. (container)");
        }
    }
}
