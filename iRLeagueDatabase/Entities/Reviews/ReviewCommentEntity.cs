using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Members;
using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Reviews
{
    [Serializable]
    public class ReviewCommentEntity : CommentBaseEntity
    {
        [ForeignKey(nameof(Review))]
        public long ReviewId { get; set; }
        public virtual IncidentReviewEntity Review { get; set; }
        
        [InverseProperty(nameof(CommentReviewVoteEntity.ReviewComment))]
        public virtual List<CommentReviewVoteEntity> CommentReviewVotes { get; set; }

        public ReviewCommentEntity () { }

        public override void Delete(LeagueDbContext dbContext)
        {
            if (CommentReviewVotes != null)
                CommentReviewVotes.ToList().ForEach(x => x.Delete(dbContext));

            base.Delete(dbContext);
        }
    }
}
