using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Reviews;
using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterReviewsTypeMaps()
        {
            RegisterTypeMap<IncidentReviewEntity, IncidentReviewInfoDTO>(MapToReviewInfoDTO);
            RegisterTypeMap<IncidentReviewEntity, IncidentReviewDataDTO>(MapToPublicReviewDataDTO);
            //RegisterTypeMap<IncidentReviewEntity, IncidentReviewDataDTO>(MapToReviewDataDTO);
            RegisterTypeMap<CommentBaseEntity, CommentInfoDTO>(MapToCommentInfoDTO);
            RegisterTypeMap<CommentBaseEntity, CommentDataDTO>(MapToCommentDataDTO);
            RegisterTypeMap<ReviewCommentEntity, ReviewCommentDataDTO>(MapToReviewCommentDataDTO);
            RegisterTypeMap<CommentReviewVoteEntity, ReviewVoteDataDTO>(MapToReviewVoteDataDTO);
            RegisterTypeMap<AcceptedReviewVoteEntity, ReviewVoteDataDTO>(MapToReviewVoteDataDTO);
            RegisterTypeMap<VoteCategoryEntity, VoteCategoryDTO>(MapToVoteCategoryDTO);
            RegisterTypeMap<CustomIncidentEntity, CustomIncidentDTO>(MapToCustomIncidentDTO);
            RegisterTypeMap<ReviewPenaltyEntity, ReviewPenaltyDTO>(MapToReviewPenaltyDTO);
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

        public IncidentReviewDataDTO MapToPublicReviewDataDTO(IncidentReviewEntity source, IncidentReviewDataDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new IncidentReviewDataDTO();
            }

            MapToReviewInfoDTO(source, target);
            //target.Author = MapToMemberInfoDTO(source.Author);
            target.IncidentKind = source.IncidentKind;
            target.FullDescription = source.FullDescription;
            target.AuthorName = source.AuthorName;
            target.Corner = source.Corner;
            target.InvolvedMemberIds = source.InvolvedMembers?.Select(x => x.MemberId).ToArray();
            target.OnLap = source.OnLap;
            target.ReviewId = source.ReviewId;
            target.SessionId = source.SessionId; //MapToSessionInfoDTO(source.Session);
            target.TimeStamp = source.TimeStamp;
            target.AcceptedReviewVotes = source.AcceptedReviewVotes?.Select(x => MapToReviewVoteDataDTO(x)).ToArray();
            target.ResultLongText = source.ResultLongText;
            target.IncidentNr = source.IncidentNr;
            target.Comments = new ReviewCommentDataDTO[0];

            return target;
        }

        public IncidentReviewDataDTO MapToReviewDataDTO(IncidentReviewEntity source, IncidentReviewDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new IncidentReviewDataDTO();

            MapToPublicReviewDataDTO(source, target);
            target.Comments = source.Comments?.Select(x => MapToReviewCommentDataDTO(x)).ToArray();

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
            target.Replies = source.Replies?.Select(x => MapToCommentDataDTO(x)).ToArray();

            return target;
        }

        public ReviewCommentDataDTO MapToReviewCommentDataDTO(ReviewCommentEntity source, ReviewCommentDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ReviewCommentDataDTO();

            MapToCommentDataDTO(source, target);
            target.ReviewId = source.ReviewId; // MapToReviewInfoDTO(source.Review);
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
            target.MemberAtFaultId = source.MemberAtFaultId; // MapToMemberInfoDTO(source.MemberAtFault);
            target.VoteCategoryId = source.CustomVoteCatId;
            target.CatText = source.CustomVoteCat?.Text;
            target.CatPenalty = source.CustomVoteCat?.DefaultPenalty ?? 0;
            target.Description = source.Description;

            return target;
        }

        public VoteCategoryDTO MapToVoteCategoryDTO(VoteCategoryEntity source, VoteCategoryDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new VoteCategoryDTO();

            target.CatId = source.CatId;
            target.Text = source.Text;
            target.DefaultPenalty = source.DefaultPenalty;
            target.Index = source.Index;

            return target;
        }

        public CustomIncidentDTO MapToCustomIncidentDTO(CustomIncidentEntity source, CustomIncidentDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new CustomIncidentDTO();

            target.IncidentId = source.IncidentId;
            target.Index = source.Index;
            target.Text = source.Text;

            return target;
        }
        public ReviewPenaltyDTO MapToReviewPenaltyDTO(ReviewPenaltyEntity source, ReviewPenaltyDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new ReviewPenaltyDTO();

            target.PenaltyPoints = source.PenaltyPoints;
            target.ResultRowId = source.ResultRowId;
            target.ReviewId = source.ReviewId;
            target.ReviewVote = MapToReviewVoteDataDTO(source.ReviewVote);
            target.IncidentNr = source.Review.IncidentNr;

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
            RegisterTypeMap<VoteCategoryDTO, VoteCategoryEntity>(MapToVoteCategoryEntity);
            RegisterTypeMap<CustomIncidentDTO, CustomIncidentEntity>(MapToCustomIncidentEntity);
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
            target.Session = GetSessionBaseEntity(new SessionInfoDTO() { SessionId = source.SessionId });
            target.IncidentKind = source.IncidentKind;
            target.FullDescription = source.FullDescription;
            target.AuthorName = source.AuthorName;
            if (target.Comments == null)
                target.Comments = new List<ReviewCommentEntity>();
            MapCollection(source.Comments, target.Comments, MapToReviewCommentEntity, x => x.CommentId,
                removeFromCollection: false, removeFromDatabase: false, autoAddMissing: false);
            target.Corner = source.Corner;
            if (target.InvolvedMembers == null)
                target.InvolvedMembers = new List<Entities.Members.LeagueMemberEntity>();
            MapCollection(source.InvolvedMemberIds.Select(x => new LeagueMemberInfoDTO() { MemberId = x }), target.InvolvedMembers, GetMemberEntity, x => x.MemberId, removeFromCollection: true);
            target.OnLap = source.OnLap;
            target.TimeStamp = source.TimeStamp;
            if (target.AcceptedReviewVotes == null)
                target.AcceptedReviewVotes = new List<AcceptedReviewVoteEntity>();
            MapCollection(source.AcceptedReviewVotes, target.AcceptedReviewVotes, MapToAcceptedReviewVoteEntity, x => x.ReviewVoteId, 
                removeFromCollection: true, removeFromDatabase: true, autoAddMissing: true);
            target.ResultLongText = source.ResultLongText;
            target.IncidentNr = source.IncidentNr;
            if (target.Session?.SessionResult != null)
            {
                target.Session.SessionResult.RequiresRecalculation = true;
            }

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
            //target.Review = GetReviewEntity(new IncidentReviewInfoDTO() { ReviewId = source.ReviewId });
            target.Review = DefaultGet<IncidentReviewEntity>(source.ReviewId);
            if (target.CommentReviewVotes == null)
                target.CommentReviewVotes = new List<CommentReviewVoteEntity>();
            MapCollection(source.CommentReviewVotes, target.CommentReviewVotes, MapToCommentReviewVoteEntity, 
                x => x.Keys, removeFromCollection: true, removeFromDatabase: true, autoAddMissing: true);

            return target;
        }

        public ReviewVoteEntity MapToReviewVoteEntity(ReviewVoteDataDTO source, ReviewVoteEntity target)
        {
            if (source == null || target == null)
                return null;

            target.MemberAtFault = DefaultGet<LeagueMemberEntity>(source.MemberAtFaultId);
            target.Vote = source.Vote;
            target.CustomVoteCat = DefaultGet<VoteCategoryEntity>(source.VoteCategoryId);
            target.Description = source.Description;

            return target;
        }

        public AcceptedReviewVoteEntity MapToAcceptedReviewVoteEntity(ReviewVoteDataDTO source, AcceptedReviewVoteEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetAcceptedReviewVoteEntity(source);

            MapToReviewVoteEntity(source, target);

            if (target.IncidentReview?.Session?.SessionResult != null)
            {
                target.IncidentReview.Session.SessionResult.RequiresRecalculation = true;
            }

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

        public VoteCategoryEntity MapToVoteCategoryEntity(VoteCategoryDTO source, VoteCategoryEntity target = null)
        {
            if (source == null)
                return null;

            // search for target in database
            if (target == null)
                DefaultGet(source, ref target);

            // if target is not in database, create
            if (target == null)
            {
                target = DbContext.CustomVoteCategories.Create();
                DbContext.CustomVoteCategories.Add(target);
            }

            target.DefaultPenalty = source.DefaultPenalty;
            target.Index = source.Index;
            target.Text = source.Text;

            return target;
        }

        public CustomIncidentEntity MapToCustomIncidentEntity(CustomIncidentDTO source, CustomIncidentEntity target = null)
        {
            if (source == null)
                return null;

            // search for target in database
            if (target == null)
                DefaultGet(source, ref target);

            // if target is not in database, create
            if (target == null)
            {
                target = DbContext.CustomIncidentKinds.Create();
                DbContext.CustomIncidentKinds.Add(target);
            }

            target.Index = source.Index;
            target.Text = source.Text;

            return target;
        }
    }
}
