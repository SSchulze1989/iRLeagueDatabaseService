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

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = 0.5;

            var test2 = test as IComparable;

            var member = new LeagueMemberEntity()
            {
                Team = new TeamEntity()
                {
                    Name = "Test Team"
                },
                Firstname = "Test",
                Lastname = "Member"
            };

            var resultRow = new ResultRowEntity()
            {
                Member = member
            };

            var propertyName = "Member.Team.TeamId";

            var nestedProperty = typeof(ResultRowEntity).GetNestedPropertyInfo(propertyName);
            var nestedValue = nestedProperty.GetValue(resultRow);

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
