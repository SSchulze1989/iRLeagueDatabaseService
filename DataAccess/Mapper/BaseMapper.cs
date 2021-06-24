using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.DataTransfer;

namespace iRLeagueDatabase.DataAccess.Mapper
{
    public partial class DTOMapper
    {
        private void RegisterBaseTypeMaps()
        {
            //Base types
            RegisterTypeMap<SeasonEntity, SeasonInfoDTO>(MapToSeasonInfoDTO);
            RegisterTypeMap<SeasonEntity, SeasonDataDTO>(MapToSeasonDataDTO);
        }

        public VersionInfoDTO MapToVersionInfoDTO(Revision source, VersionInfoDTO target)
        {
            if (source == null)
                return null;

            target.CreatedOn = source.CreatedOn;
            target.LastModifiedOn = source.LastModifiedOn;
            target.Version = source.Version;
            target.CreatedByUserId = source.CreatedByUserId;
            target.CreatedByUserName = source.CreatedByUserName;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.LastModifiedByUserName = source.LastModifiedByUserName;
            return target;
        }

        public VersionDTO MapToVersionDTO(Revision source, VersionDTO target)
        {
            if (source == null)
                return null;

            MapToVersionInfoDTO(source, target);
            target.CreatedByUserId = target.CreatedByUserId;
            target.LastModifiedByUserId = target.LastModifiedByUserId;
            return target;
        }

        public SeasonInfoDTO MapToSeasonInfoDTO(SeasonEntity source, SeasonInfoDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new SeasonInfoDTO();

            MapToVersionInfoDTO(source, target);
            target.SeasonId = source.SeasonId;
            target.SeasonName = source.SeasonName;

            return target;
        }

        public SeasonDataDTO MapToSeasonDataDTO(SeasonEntity source, SeasonDataDTO target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = new SeasonDataDTO();

            MapToSeasonInfoDTO(source, target);
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            target.Finished = source.Finished;
            target.Results = source.Results?.Select(x => MapToResultInfoDTO(x)).ToArray();
            target.Reviews = source.Results?.Select(x => x.Reviews?.Select(y => MapToReviewInfoDTO(y))).Aggregate((x, y) => x.Concat(y)).ToArray();
            target.Schedules = source.Schedules?.Select(x => MapToScheduleInfoDTO(x)).ToArray();
            target.Scorings = source.Scorings?.Select(x => MapToScoringDataDTO(x)).ToArray();
            target.ScoringTables = source.ScoringTables?.Select(x => MapToScoringTableDataDTO(x)).ToArray();
            target.SeasonEnd = source.SeasonEnd.GetValueOrDefault();
            target.SeasonId = source.SeasonId;
            target.SeasonName = source.SeasonName;
            target.SeasonStart = source.SeasonStart.GetValueOrDefault();
            target.Version = source.Version;
            target.VoteCategories = LeagueDbContext.CustomVoteCategories.AsEnumerable().Select(x => MapToVoteCategoryDTO(x)).ToArray();
            target.CustomIncidents = LeagueDbContext.CustomIncidentKinds.AsEnumerable().Select(x => MapToCustomIncidentDTO(x)).ToArray();
            target.HideCommentsBeforeVoted = source.HideCommentsBeforeVoted;
            target.SeasonStatisticSetIds = source.SeasonStatistics.Select(x => x.Id).ToArray();

            return target;
        }
    }

    public partial class EntityMapper
    {
        private void RegisterBaseTypeMaps()
        {
            //Base types
            RegisterTypeMap<SeasonDataDTO, SeasonEntity>(MapToSeasonEntity);
            RegisterTypeMap<SeasonInfoDTO, SeasonEntity>(MapToSeasonEntity);
        }

        /// <summary>
        /// Checks if source version is newer than target version and maps 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool MapToRevision(VersionInfoDTO source, Revision target)
        {
            if (source == null)
                return false;
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Version = source.Version;

            if (target.CreatedOn == null)
            {
                if (source.CreatedOn != null)
                    target.CreatedOn = source.CreatedOn;
                else
                    target.CreatedOn = DateTime.Now;

                target.CreatedByUserId = UserId;
                target.CreatedByUserName = UserName;
            }
            if (target.LastModifiedOn == null || source.LastModifiedOn >= target.LastModifiedOn || ForceOldVersion)
            {
                //target.CreatedOn = source.CreatedOn;
                target.LastModifiedOn = source.LastModifiedOn;
                target.LastModifiedByUserName = UserName;
                target.LastModifiedByUserId = UserId;

                return true;
            }
            else
                return false;
        }

        public SeasonEntity GetSeasonEntity(SeasonInfoDTO source)
        {
            //if (source == null)
            //    return null;
            //SeasonEntity target;

            //if (source.SeasonId == null)
            //    target = new SeasonEntity();
            //else
            //    target = DbContext.Seasons.Find(source.SeasonId);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(SeasonEntity), "Could not find Entity in Database.", source.SeasonId);

            //return target;
            return DefaultGet<SeasonInfoDTO, SeasonEntity>(source);
        }
        public SeasonEntity MapToSeasonEntity(SeasonInfoDTO source, SeasonEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
            {
                GetSeasonEntity(source);
            }

            if (!MapToRevision(source, target))
                return target;

            target.SeasonName = source.SeasonName;

            return target;
        }

        public SeasonEntity MapToSeasonEntity(SeasonDataDTO source, SeasonEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
            {
                GetSeasonEntity(source);
            }

            if (!MapToRevision(source, target))
                return target;

            target.SeasonName = source.SeasonName;
            target.Finished = source.Finished;
            target.CreatedByUserId = source.CreatedByUserId;
            target.LastModifiedByUserId = source.LastModifiedByUserId;
            if (target.Schedules == null)
                target.Schedules = new List<Entities.Sessions.ScheduleEntity>();
            MapCollection(source.Schedules, target.Schedules, GetScheduleEntity, x => x.MappingId, autoAddMissing: true);
            if (target.Scorings == null)
                target.Scorings = new List<Entities.Results.ScoringEntity>();
            MapCollection(source.Scorings, target.Scorings, MapToScoringEntity, x => x.ScoringId, autoAddMissing: true);
            if (target.ScoringTables == null)
                target.ScoringTables = new List<Entities.Results.ScoringTableEntity>();
            MapCollection(source.ScoringTables, target.ScoringTables, MapToScoringTableEntity, x => x.ScoringTableId, autoAddMissing: true);
            target.SeasonName = source.SeasonName;
            target.Version = source.Version;
            target.HideCommentsBeforeVoted = source.HideCommentsBeforeVoted;

            return target;
        }
    }
}
