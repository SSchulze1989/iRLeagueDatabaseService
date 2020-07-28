﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;


namespace iRLeagueDatabase.Entities.Reviews
{
    [Serializable]
    public class IncidentReviewEntity : Revision
    {
        [Key]
        public long ReviewId { get; set; }

        public override object MappingId => ReviewId;

        //public virtual ResultEntity Result { get; set; }

        [ForeignKey(nameof(Session))]
        public long SessionId { get; set; }
        //public virtual SessionBaseEntity Session { get; set; }
        public virtual SessionBaseEntity Session { get; set; }

        //[ForeignKey(nameof(Author))]
        //public int AuthorId { get; set; }
        //public virtual LeagueMemberEntity Author { get; set; }
        public string AuthorUserId { get; set; }
        public string AuthorName { get; set; }

        public string IncidentKind { get; set; }

        public string FullDescription { get; set; }

        public int OnLap { get; set; }
        
        public int Corner { get; set; }
        
        public TimeSpan TimeStamp { get; set; }
        
        public virtual List<LeagueMemberEntity> InvolvedMembers { get; set; }
        
        [InverseProperty(nameof(ReviewCommentEntity.Review))]
        public virtual List<ReviewCommentEntity> Comments { get; set; }

        [InverseProperty(nameof(AcceptedReviewVoteEntity.IncidentReview))]
        public virtual List<AcceptedReviewVoteEntity> AcceptedReviewVotes { get; set; }

        //public VoteState VoteState { get; set; }

        public IncidentReviewEntity()
        {
            InvolvedMembers = new List<LeagueMemberEntity>();
            Comments = new List<ReviewCommentEntity>();
        }

        public IncidentReviewEntity(LeagueMemberEntity author) : this()
        {
            //Author = author;
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            if (Comments != null && Comments.Count() > 0)
                Comments.ToList().ForEach(x => x.Delete(dbContext));

            if (AcceptedReviewVotes != null)
                AcceptedReviewVotes.ToList().ForEach(x => x.Delete(dbContext));

            base.Delete(dbContext);
        }
    }
}
