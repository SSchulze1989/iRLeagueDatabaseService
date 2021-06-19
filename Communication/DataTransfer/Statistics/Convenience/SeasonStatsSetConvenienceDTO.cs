using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Statistics.Convenience
{
    [DataContract]
    public class SeasonStatsSetConvenienceDTO : StatisticConvenienceDTO
    {
        [DataMember]
        public long SeasonId { get; set; }
        [DataMember]
        public long SeasonName { get; set; }
    }
}
