using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews.Convenience
{
    [DataContract]
    public class MemberPenaltySummaryDTO : BaseDTO
    {
        [DataMember]
        public long MemberId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public int Points { get; set; }
        [DataMember]
        public ReviewVoteDataDTO[] Penalties { get; set; }
    }
}
