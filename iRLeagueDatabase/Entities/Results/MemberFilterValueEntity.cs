using iRLeagueDatabase.Entities.Members;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Results
{
    public class MemberFilterValueEntity : FilterValueBaseEntity
    {
        [ForeignKey(nameof(MemberValue)), Column("MemberIdValue")]
        public long MemberId { get; set; }
        public LeagueMemberEntity MemberValue { get; set; }
        public override object Value { get => MemberValue; set => MemberValue = (LeagueMemberEntity)value; }
        public override Type GetValueType()
        {
            return typeof(LeagueMemberEntity);
        }
    }
}
