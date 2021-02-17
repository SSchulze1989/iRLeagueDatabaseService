using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews
{
    public class MemberPenaltySummaryDTO : BaseDTO
    {
        public long MemberId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int Points { get; set; }
        public ReviewVoteDataDTO[] Penalties { get; set; }
    }
}
