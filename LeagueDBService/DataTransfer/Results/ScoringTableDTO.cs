using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using iRLeagueDatabase.DataTransfer.Sessions;

namespace iRLeagueDatabase.DataTransfer.Results
{
    public class ScoringTableDataDTO : ScoringTableInfoDTO
    {
        public int DropWeeks { get; set; }
        public int AverageRaceNr { get; set; }
        public SeasonInfoDTO Season { get; set; }
        public string ScoringFactors { get; set; }
        public ScoringInfoDTO[] Scorings { get; set; }
        public SessionInfoDTO[] Sessions { get; set; }
    }
}
