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