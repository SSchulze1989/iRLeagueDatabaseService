﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.DataTransfer.Members
{
    [DataContract(Name = "LeagueMemberInfoDTO")]
    //[KnownType(typeof(LeagueMemberDataDTO))]
    public class LeagueMemberInfoDTO : VersionInfoDTO, IMappableDTO
    {
        [DataMember]
        public long? MemberId { get; set; }

        public override object MappingId => MemberId;

        public override object[] Keys => new object[] { MemberId };

        //object IMappableDTO.MappingId => MemberId;
    }
}
