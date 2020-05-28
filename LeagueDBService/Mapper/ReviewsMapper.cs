using System;
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
        }

        public IncidentReviewInfoDTO MapToReviewInfoDTO(IncidentReviewEntity source, IncidentReviewInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new IncidentReviewInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.ReviewId = source.ReviewId;

            return target;
        }

        public IncidentReviewDataDTO MapToReviewDataDTO(IncidentReviewEntity source, IncidentReviewDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new IncidentReviewDataDTO();

            MapToReviewInfoDTO(source, target);
            target.Author = MapToMemberInfoDTO(source.Author);
            target.Comments = source.Comments.Select(x => MapToReviewCommentDataDTO(x)).ToList();
            target.Corner = source.Corner;
            target.InvolvedMembers = source.InvolvedMembers.Select(x => MapToMemberInfoDTO(x)).ToList();
            target.OnLap = source.OnLap;
            target.ReviewId = source.ReviewId;
            target.Session = MapToSessionInfoDTO(source.Session);
            target.TimeStamp = source.TimeStamp;

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

            return target;
        }

        public CommentDataDTO MapToCommentDataDTO(CommentBaseEntity source, CommentDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new CommentDataDTO();

            MapToCommentInfoDTO(source, target);
            target.Author = MapToMemberInfoDTO(source.Author);
            target.Date = source.Date;
            target.Text = source.Text;

            return target;
        }

        public ReviewCommentDataDTO MapToReviewCommentDataDTO(ReviewCommentEntity source, ReviewCommentDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ReviewCommentDataDTO();

            MapToCommentDataDTO(source, target);
            target.MemberAtFault = MapToMemberInfoDTO(source.MemberAtFault);
            target.Vote = source.Vote;

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
        }

        public IncidentReviewEntity GetReviewEntity(IncidentReviewInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //IncidentReviewEntity target;

            //if (source.ReviewId == null)
            //    target = new IncidentReviewEntity();
            //else
            //    target = DbContext.Set<IncidentReviewEntity>().Find(source.ReviewId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(IncidentReviewEntity), "Could not find Entity in Database.", source.ReviewId);

            //return target;
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

            target.Author = GetMemberEntity(source.Author);
            MapCollection(source.Comments, target.Comments, MapToReviewCommentEntity, x => x.CommentId);
            target.Corner = source.Corner;
            MapCollection(source.InvolvedMembers, target.InvolvedMembers, GetMemberEntity, x => x.MemberId);
            target.OnLap = source.OnLap;
            target.TimeStamp = source.TimeStamp;

            return target;
        }

        public ReviewCommentEntity GetReviewCommentEntity(ReviewCommentDataDTO source)
        {
            //if (source == null)
            //    return null;
            //ReviewCommentEntity target;

            //if (source.CommentId == null)
            //    target = new ReviewCommentEntity();
            //else
            //    target = DbContext.Set<ReviewCommentEntity>().Find(source.CommentId);
            //if (target == null)
            //    throw new EntityNotFoundException(nameof(ReviewCommentEntity), "Could not find Entity in Database.", source.CommentId);

            //return target;
            return DefaultGet<ReviewCommentDataDTO, ReviewCommentEntity>(source);
        }

        public CommentBaseEntity GetCommentEntity(CommentDataDTO source)
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
            return DefaultGet<CommentDataDTO, CommentBaseEntity>(source);
        }

        public CommentBaseEntity MapToCommentBaseEntity(CommentDataDTO source, CommentBaseEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetCommentEntity(source);

            if (!MapToRevision(source, target))
                return target;

            target.Author = GetMemberEntity(source.Author);
            target.Date = source.Date;
            target.Text = source.Text;

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
            target.MemberAtFault = GetMemberEntity(source.MemberAtFault);
            target.Vote = source.Vote;

            return target;
        }
    }
}
