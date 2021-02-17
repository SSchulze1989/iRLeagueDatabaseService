using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews
{
    [DataContract]
    public class ReviewsPenaltySummaryDTO : BaseDTO
    {
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public int Points { get; set; }
        [DataMember]
        public MemberPenaltySummaryDTO[] DrvPenalties { get; set; }
    }
}
