using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer.Reviews
{
    [Serializable]
    [DataContract]
    public class PublicIncidentReviewDataDTO : IncidentReviewInfoDTO
    {
        public override Type Type => typeof(IncidentReviewDataDTO);

        //[DataMember]
        //public int ReviewId { get; set; }
        //[DataMember]
        //public int SeasonId;
        //[DataMember]
        //public LeagueMemberDTO Author { get; set; }
        [DataMember]
        //public SessionInfoDTO Session { get; set; }
        public long SessionId { get; set; }
        [DataMember]
        public string IncidentNr { get; set; }
        [DataMember]
        public string IncidentKind { get; set; }
        [DataMember]
        public string FullDescription { get; set; }
        //[DataMember]
        //public LeagueMemberInfoDTO Author { get; set; }
        [DataMember]
        public string OnLap { get; set; }
        [DataMember]
        public string Corner { get; set; }
        [DataMember]
        public TimeSpan TimeStamp { get; set; }
        [DataMember]
        public long[] InvolvedMemberIds { get; set; }
        [DataMember]
        public ReviewVoteDataDTO[] AcceptedReviewVotes { get; set; }

        [DataMember]
        public string ResultLongText { get; set; }
        //[DataMember]
        //public LeagueMemberInfoDTO MemberAtFault { get; set; }
        //[DataMember]
        //public VoteEnum VoteResult { get; set; }
        //[DataMember]
        //public VoteState VoteState { get; set; }

        //[DataMember]
        //public LeagueMemberInfoDTO CreatedBy { get; set; }
        //[DataMember]
        //public LeagueMemberInfoDTO LastModifiedBy { get; set; }

        #region Version Info
        [DataMember]
        public new DateTime? CreatedOn { get => base.CreatedOn; set => base.CreatedOn = value; }
        [DataMember]
        public new DateTime? LastModifiedOn { get => base.LastModifiedOn; set => base.LastModifiedOn = value; }
        [DataMember]
        public new string CreatedByUserId { get => base.CreatedByUserId; set => base.CreatedByUserId = value; }
        [DataMember]
        public new string LastModifiedByUserId { get => base.LastModifiedByUserId; set => base.LastModifiedByUserId = value; }
        [DataMember]
        public new string CreatedByUserName { get => base.CreatedByUserName; set => base.CreatedByUserName = value; }
        [DataMember]
        public new string LastModifiedByUserName { get => base.LastModifiedByUserName; set => base.LastModifiedByUserName = value; }
        #endregion
    }
}
