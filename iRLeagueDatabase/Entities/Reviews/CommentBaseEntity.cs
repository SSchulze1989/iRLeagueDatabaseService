﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities.Reviews
{
    [Serializable]
    public class CommentBaseEntity : Revision
    {
        [Key]
        public long CommentId { get; set; }

        public override object MappingId => CommentId;

        public DateTime? Date { get; set; }

        public string AuthorUserId { get; set; }
        //[ForeignKey(nameof(Author))]
        //public int AuthorId { get; set; }
        //public LeagueMemberEntity Author { get; set; }

        public string AuthorName { get; set; }

        public string Text { get; set; }

        [ForeignKey(nameof(ReplyTo))]
        public long? ReplyToCommentId { get; set; }
        public virtual CommentBaseEntity ReplyTo { get; set; }

        [InverseProperty(nameof(CommentBaseEntity.ReplyTo))]
        public virtual List<CommentBaseEntity> Replies { get; set; }

        public CommentBaseEntity()
        {
            Date = DateTime.Now;
        }

        public override void Delete(LeagueDbContext dbContext)
        {
            Replies?.ToList().ForEach(x => x.Delete(dbContext));

            base.Delete(dbContext);
        }
    }
}
