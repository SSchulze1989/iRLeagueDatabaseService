using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace iRLeagueRESTService.Models
{
    [Serializable()]
    public class LeagueRegister
    {
        private static string path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Leagues/Register.xml");
        public List<LeagueEntry> Leagues { get; set; }

        public LeagueRegister()
        {
            Leagues = new List<LeagueEntry>();
        }

        public static LeagueRegister Get()
        {
            var serializer = new XmlSerializer(typeof(LeagueRegister));

            if (File.Exists(path) == false)
            {
                var newRegister = new LeagueRegister();
                using (var stream = new StreamWriter(path))
                {
                    serializer.Serialize(stream, newRegister);
                }
            }

            LeagueRegister register = null;
            using (var stream = new StreamReader(path))
            {
                register = serializer.Deserialize(stream) as LeagueRegister;
            }

            return register;
        }

        /// <summary>
        /// Get a league enty from the register by its league name
        /// </summary>
        /// <param name="leagueName">Shortname of the league</param>
        /// <returns><see cref="LeagueEntry"/> of the league; <see langword="null"/> if not found;</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public LeagueEntry GetLeague(string leagueName)
        {
            // check if league exists
            if (Leagues.Any(x => x.Name == leagueName) == false)
            {
                return null;
            }

            // retrieve entry from the register
            return Leagues.Single(x => x.Name == leagueName);
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(LeagueRegister));

            using (var stream = new StreamWriter(path))
            {
                serializer.Serialize(stream, this);
            }
        }
    }
}