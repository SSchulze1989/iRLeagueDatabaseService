﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.DataTransfer.Reviews;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterReviewsTypeMaps()
        {
            RegisterTypeMap<IncidentReviewEntity, IncidentReviewInfoDTO>(MapToReviewInfoDTO);
            RegisterTypeMap<IncidentReviewEntity, IncidentReviewDataDTO>(MapToReviewDataDTO);
            RegisterTypeMap<CommentBaseEntity, CommentInfoDTO>(MapToCommentInfoDTO);
            RegisterTypeMap<CommentBaseEntity, CommentDataDTO>(MapToCommentDataDTO);
            RegisterTypeMap<ReviewCommentEntity, ReviewCommentDataDTO>(MapToReviewCommentDataDTO);
            RegisterTypeMap<CommentReviewVoteEntity, ReviewVoteDataDTO>(MapToReviewVoteDataDTO);
            RegisterTypeMap<AcceptedReviewVoteEntity, ReviewVoteDataDTO>(MapToReviewVoteDataDTO);
        }

        public IncidentReviewInfoDTO MapToReviewInfoDTO(IncidentReviewEntity source, IncidentReviewInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new IncidentReviewInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.ReviewId = source.ReviewId;
            target.AuthorName = source.AuthorName;
            target.AuthorUserId = source.AuthorUserId;

            return target;
        }

        public IncidentReviewDataDTO MapToReviewDataDTO(IncidentReviewEntity source, IncidentReviewDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new IncidentReviewDataDTO();

            MapToReviewInfoDTO(source, target);
            //target.Author = MapToMemberInfoDTO(source.Author);
            target.IncidentKind = source.IncidentKind;
            target.FullDescription = source.FullDescription;
            target.AuthorName = source.AuthorName;
            target.Comments = source.Comments.Select(x => MapToReviewCommentDataDTO(x)).ToList();
            target.Corner = source.Corner;
            target.InvolvedMembers = source.InvolvedMembers.Select(x => MapToMemberInfoDTO(x)).ToList();
            target.OnLap = source.OnLap;
            target.ReviewId = source.ReviewId;
            target.Session = MapToSessionInfoDTO(source.Session);
            target.TimeStamp = source.TimeStamp;
            target.AcceptedReviewVotes = source.AcceptedReviewVotes.Select(x => MapToReviewVoteDataDTO(x)).ToArray();

            return target;
        }

        public CommentInfoDTO MapToCommentInfoDTO(CommentBaseEntity source, CommentInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new CommentInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.CommentId = source.CommentId;
            target.AuthorUserId = source.AuthorUserId;
            target.AuthorName = source.AuthorName;

            return target;
        }

        public CommentDataDTO MapToCommentDataDTO(CommentBaseEntity source, CommentDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new CommentDataDTO();

            MapToCommentInfoDTO(source, target);
            //target.Author = MapToMemberInfoDTO(source.Author);
            target.AuthorName = source.AuthorName;
            target.Date = source.Date;
            target.Text = source.Text;
            target.ReplyTo = MapToCommentInfoDTO(source.ReplyTo);
            target.Replies = source.Replies.Select(x => MapToCommentDataDTO(x)).ToArray();

            return target;
        }

        public ReviewCommentDataDTO MapToReviewCommentDataDTO(ReviewCommentEntity source, ReviewCommentDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ReviewCommentDataDTO();

            MapToCommentDataDTO(source, target);
            target.Review = MapToReviewInfoDTO(source.Review);
            target.CommentReviewVotes = source.CommentReviewVotes.Select(x => MapToReviewVoteDataDTO(x)).ToArray();

            return target;
        }

        public ReviewVoteDataDTO MapToReviewVoteDataDTO(ReviewVoteEntity source, ReviewVoteDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ReviewVoteDataDTO();

            target.ReviewVoteId = source.ReviewVoteId;
            target.Vote = source.Vote;
            target.MemberAtFault = MapToMemberInfoDTO(source.MemberAtFault);

            return target;
        }
    }

    public partial class EntityMapper
    {
        public void RegisterReviewsTypeMaps()
        {
            RegisterTypeMap<IncidentReviewDataDTO, IncidentReviewEntity>(MapToReviewEntity);
            RegisterTypeMap<CommentDataDTO, CommentBaseEntity>(MapToCommentBaseEntity);
            RegisterTypeMap<ReviewCommentDataDTO, ReviewCommentEntity>(MapToReviewCommentEntity);
            RegisterTypeMap<ReviewVoteDataDTO, ReviewVoteEntity>(MapToReviewVoteEntity);
        }

        public IncidentReviewEntity GetReviewEntity(IncidentReviewInfoDTO source)
        {
            return DefaultGet<IncidentReviewInfoDTO, IncidentReviewEntity>(source);
        }

        public IncidentReviewEntity MapToReviewEntity(IncidentReviewDataDTO source, IncidentReviewEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetReviewEntity(source);

            if (!MapToRevision(source, target))
                return target;

            //target.Author = GetMemberEntity(source.Author);
            target.AuthorUserId = source.AuthorUserId;
            target.Session = GetSessionBaseEntity(source.Session);
            target.IncidentKind = source.IncidentKind;
            target.FullDescription = source.FullDescription;
            target.AuthorName = source.AuthorName;
            if (target.Comments == null)
                target.Comments = new List<ReviewCommentEntity>();
            MapCollection(source.Comments, target.Comments, MapToReviewCommentEntity, x => x.CommentId);
            target.Corner = source.Corner;
            if (target.InvolvedMembers == null)
                target.InvolvedMembers = new List<Entities.Members.LeagueMemberEntity>();
            MapCollection(source.InvolvedMembers, target.InvolvedMembers, GetMemberEntity, x => x.MemberId, removeFromCollection: true);
            target.OnLap = source.OnLap;
            target.TimeStamp = source.TimeStamp;
            if (target.AcceptedReviewVotes == null)
                target.AcceptedReviewVotes = new List<AcceptedReviewVoteEntity>();
            MapCollection(source.AcceptedReviewVotes, target.AcceptedReviewVotes, MapToAcceptedReviewVoteEntity, x => x.ReviewVoteId, removeFromCollection: true, removeFromDatabase: true);

            return target;
        }

        public ReviewCommentEntity GetReviewCommentEntity(ReviewCommentDataDTO source)
        {
            return DefaultGet<ReviewCommentDataDTO, ReviewCommentEntity>(source);
        }

        public CommentBaseEntity GetCommentEntity(CommentInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //CommentBaseEntity target;

            //if (source is ReviewCommentDataDTO reviewComment)
            //    target = GetReviewCommentEntity(reviewComment);
            //else if (source.CommentId == null)
            //    target = new CommentBaseEntity();
            //else
            //    target = DbContext.Set<CommentBaseEntity>().Find(source.CommentId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(CommentBaseEntity), "Could not find Entity in Database.", source.CommentId);

            //return target;
            return DefaultGet<CommentInfoDTO, CommentBaseEntity>(source);
        }

        public AcceptedReviewVoteEntity GetAcceptedReviewVoteEntity(ReviewVoteDataDTO source)
        {
            var target = DefaultGet<ReviewVoteDataDTO, AcceptedReviewVoteEntity>(source);

            if (target == null)
                target = new AcceptedReviewVoteEntity();

            return target;
        }

        public CommentReviewVoteEntity GetCommentReviewVoteEntity(ReviewVoteDataDTO source)
        {
            var target = DefaultGet<ReviewVoteDataDTO, CommentReviewVoteEntity>(source);

            if (target == null)
                target = new CommentReviewVoteEntity();

            return target;
        }

        public CommentBaseEntity MapToCommentBaseEntity(CommentDataDTO source, CommentBaseEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetCommentEntity(source);

            if (!MapToRevision(source, target))
                return target;

            //target.Author = GetMemberEntity(source.Author);
            target.AuthorUserId = source.AuthorUserId;
            target.AuthorName = source.AuthorName;
            target.Date = source.Date;
            target.Text = source.Text;
            target.ReplyTo = GetCommentEntity(source.ReplyTo);
            if (target.Replies == null)
                target.Replies = new List<CommentBaseEntity>();
            MapCollection(source.Replies, target.Replies, MapToCommentBaseEntity, x => x.Keys);

            return target;
        }

        public ReviewCommentEntity MapToReviewCommentEntity(ReviewCommentDataDTO source, ReviewCommentEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetReviewCommentEntity(source);

            if (!MapToRevision(source, target))
                return target;

            MapToCommentBaseEntity(source, target);
            target.Review = GetReviewEntity(source.Review);
            if (target.CommentReviewVotes == null)
                target.CommentReviewVotes = new List<CommentReviewVoteEntity>();
            MapCollection(source.CommentReviewVotes, target.CommentReviewVotes, MapToCommentReviewVoteEntity, x => x.Keys, removeFromCollection: true, removeFromDatabase: true);

            return target;
        }

        public ReviewVoteEntity MapToReviewVoteEntity(ReviewVoteDataDTO source, ReviewVoteEntity target)
        {
            if (source == null || target == null)
                return null;

            target.MemberAtFault = GetMemberEntity(source.MemberAtFault);
            target.Vote = source.Vote;

            return target;
        }

        public AcceptedReviewVoteEntity MapToAcceptedReviewVoteEntity(ReviewVoteDataDTO source, AcceptedReviewVoteEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetAcceptedReviewVoteEntity(source);

            MapToReviewVoteEntity(source, target);

            return target;
        }

        public CommentReviewVoteEntity MapToCommentReviewVoteEntity(ReviewVoteDataDTO source, CommentReviewVoteEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetCommentReviewVoteEntity(source);

            MapToReviewVoteEntity(source, target);

            return target;
        }
    }
}
