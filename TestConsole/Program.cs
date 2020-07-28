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
using iRLeagueDatabase.Mapper;
//using TestConsole.LeagueDBServiceRef;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.Entities.Members;
using System.Net.Http;
using iRLeagueDatabase.DataTransfer.Messages;


namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new LeagueDbContext("TestDatabase"))
            {
                var user = context.Users.First();

                Console.WriteLine("Passwort:");
                string pw = null;

                while (pw != "x")
                {
                    pw = Console.ReadLine();
                    var pwBytes = Encoding.UTF8.GetBytes(pw);

                    Console.WriteLine(user.CheckCredentials(pwBytes));
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
