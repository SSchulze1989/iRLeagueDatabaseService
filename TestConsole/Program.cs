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

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new LeagueDbContext("TestDatabase"))
            {
                var table = context.Set<ScoringTableEntity>().Find(1);
                //var scoring = new ScoringEntity[] { context.Set<ScoringEntity>().Find(5), context.Set<ScoringEntity>().Find(6) };

                //table.Scorings.AddRange(scoring);
                //table.ScoringFactors = "1;1";
                var standings = table.GetSeasonStandings();

                foreach (var row in standings.StandingsRows)
                {
                    Console.WriteLine($"{row.Position} - {row.Member.Fullname} - {row.TotalPoints}");
                }
            }

            Console.Read();
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
