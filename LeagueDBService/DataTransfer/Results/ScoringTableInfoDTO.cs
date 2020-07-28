using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Results
{
    public class ScoringTableInfoDTO : VersionInfoDTO
    {
        public long ScoringTableId { get; set; }
        public override object MappingId => ScoringTableId;
        public string Name { get; set; }
        public override object[] Keys => new object[] { ScoringTableId };
    }
}
