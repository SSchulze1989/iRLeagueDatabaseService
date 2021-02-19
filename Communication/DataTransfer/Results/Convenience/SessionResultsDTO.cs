using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Results.Convenience
{
    /// <summary>
    /// Convencience DTO for easy access to all scored results of a session
    /// Might also include the raw results data
    /// </summary>
    public class SessionResultsDTO : BaseDTO
    {
        /// <summary>
        /// Id of the session
        /// </summary>
        public long SessionId { get; set; }
        /// <summary>
        /// Number of the race in this season in chronological order (only for race sessions)
        /// </summary>
        public int RaceNr { get; set; }
        /// <summary>
        /// Number of scored results
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Scored results data
        /// </summary>
        public ScoredResultDataDTO[] ScoredResults { get; set; }
        /// <summary>
        /// Raw results data (if included)
        /// </summary>
        public ResultDataDTO RawResults { get; set; }
        /// <summary>
        /// Sim session details
        /// </summary>
        public SimSessionDetailsDTO SessionDetails { get; set; }
    }
}
