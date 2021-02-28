using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Statistics.Special
{
    [DataContract]
    public class StatisticRowCSV : DriverStatisticRowDTO
    {
        [DataMember]
        public double FairPlayRating { get; set; }
        [DataMember]
        public string DriverRank { get; set; }
        [DataMember]
        public int RankValue { get; set; }
    }
}
