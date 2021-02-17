using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews
{
    public class ReviewsPenaltySummaryDTO : BaseDTO
    {
        public int Count { get; set; }
        public int Points { get; set; }
        public MemberPenaltySummaryDTO[] DrvPenalties { get; set; }
    }
}
