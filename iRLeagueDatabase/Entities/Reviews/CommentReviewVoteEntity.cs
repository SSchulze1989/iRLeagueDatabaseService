using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Reviews
{
    public class CommentReviewVoteEntity : ReviewVoteEntity
    {
        [ForeignKey(nameof(ReviewComment))]
        public long CommentId { get; set; }
        public virtual ReviewCommentEntity ReviewComment { get; set; }

        public override long GetLeagueId()
        {
            return ReviewComment.GetLeagueId();
        }
    }
}
