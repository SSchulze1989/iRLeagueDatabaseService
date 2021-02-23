using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace iRLeagueRESTService.Models
{
    [Serializable()]
    public class LeagueEntry
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("pretty_name")]
        public string PrettyName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorName { get; set; }
        public DateTime? LastUpdate { get; set; }
        public Guid? OwnerId { get; set; }
    }
}