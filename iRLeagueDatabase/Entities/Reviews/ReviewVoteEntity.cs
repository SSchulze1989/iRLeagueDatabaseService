using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Members;
using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Reviews
{
    public class ReviewVoteEntity : MappableEntity
    {
        [Key]
        public long ReviewVoteId { get; set; }
        [NotMapped]
        public override object MappingId => ReviewVoteId;

        [ForeignKey(nameof(MemberAtFault))]
        public long? MemberAtFaultId { get; set; }
        public virtual LeagueMemberEntity MemberAtFault { get; set; }
        public VoteEnum Vote { get; set; }
    }
}
