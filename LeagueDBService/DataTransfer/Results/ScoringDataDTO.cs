﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using iRLeagueManager.Enums;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Members;

namespace iRLeagueDatabase.DataTransfer.Results
{
    [DataContract]
    public class ScoringDataDTO : ScoringInfoDTO
    {
        public override Type Type => typeof(ScoringDataDTO);

        //[DataMember]
        //public int ScoringId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public long ScoringId { get; set; }
        [DataMember]
        public int DropWeeks { get; set; }
        [DataMember]
        public int AverageRaceNr { get; set; }
        [DataMember]
        public virtual List<SessionInfoDTO> Sessions { get; set; }
        [DataMember]
        public long SeasonId { get; set; }
        [DataMember]
        public virtual SeasonEntity Season { get; set; }
        [DataMember]
        public string BasePoints { get; set; }
        [DataMember]
        public string BonusPoints { get; set; }
        [DataMember]
        public string IncPenaltyPoints { get; set; }
        [DataMember]
        public string MultiScoringFactors { get; set; }
        [DataMember]
        public virtual List<ScoringInfoDTO> MultiScoringResults { get; set; }

        public ScoringDataDTO() { }
    }
}
