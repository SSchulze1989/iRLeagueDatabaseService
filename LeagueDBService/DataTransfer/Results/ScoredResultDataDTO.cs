using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    public class ScoredResultDataDTO : ResultDataDTO, IMappableDTO
    {
        [DataMember]
        public ScoringInfoDTO Scoring { get; set; }
        public override object MappingId => new { ResultId = ResultId.GetValueOrDefault(), ScoringId = Scoring.ScoringId.GetValueOrDefault() };

        public override object[] Keys => new object[] { ResultId, Scoring.ScoringId };

        public override Type Type => typeof(ScoredResultDataDTO);
        //object IMappableDTO.MappingId => MappingId;
        [DataMember]
        public IEnumerable<ScoredResultRowDataDTO> FinalResults { get; set; }
    }
}
