using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.DataTransfer.Members;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterMemberTypeMaps()
        {
            RegisterTypeMap<LeagueMemberEntity, LeagueMemberInfoDTO>(MapToMemberInfoDTO);
            RegisterTypeMap<LeagueMemberEntity, LeagueMemberDataDTO>(MapToMemberDataDTO);
            RegisterTypeMap<TeamEntity, TeamDataDTO>(MapToTeamDataDTO);
        }

        public LeagueMemberInfoDTO MapToMemberInfoDTO(LeagueMemberEntity source, LeagueMemberInfoDTO target = null)
        {
            if (source == null)
                return null;

            if (target == null)
                target = new LeagueMemberInfoDTO();

            target.MemberId = source.MemberId;
            return target;
        }

        public LeagueMemberDataDTO MapToMemberDataDTO(LeagueMemberEntity source, LeagueMemberDataDTO target = null)
        {
            if (source == null)
                return null;

            if (target == null)
                target = new LeagueMemberDataDTO();

            MapToMemberInfoDTO(source, target);
            target.DanLisaId = source.DanLisaId;
            target.DiscordId = source.DiscordId;
            target.Firstname = source.Firstname;
            target.IRacingId = source.IRacingId;
            target.Lastname = source.Lastname;
            target.MemberId = source.MemberId;
            //target.TeamId = source.TeamId;
            target.Team = MapToTeamDataDTO(source.Team);

            return target;
        }

        public TeamDataDTO MapToTeamDataDTO(TeamEntity source, TeamDataDTO target = null)
        {
            if (source == null)
                return null;

            if (target == null)
                target = new TeamDataDTO();

            MapToVersionInfoDTO(source, target);

            target.Name = source.Name;
            target.Profile = source.Profile;
            target.TeamColor = source.TeamColor;
            target.TeamHomepage = source.TeamHomepage;
            target.TeamId = source.TeamId;
            target.MemberIds = source.Members.Select(x => x.MemberId).ToArray();

            return target;
        }
    }

    public partial class EntityMapper
    {
        private void RegisterMemberTypeMaps()
        {
            RegisterTypeMap<LeagueMemberDataDTO, LeagueMemberEntity>(MapToMemberEntity);
            RegisterTypeMap<TeamDataDTO, TeamEntity>(MapToTeamEntity);
        }
        public LeagueMemberEntity GetMemberEntity(LeagueMemberInfoDTO source)
        {
            return DefaultGet<LeagueMemberInfoDTO, LeagueMemberEntity>(source);
        }

        public LeagueMemberEntity MapToMemberEntity(LeagueMemberDataDTO source, LeagueMemberEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = GetMemberEntity(source);

            target.DanLisaId = source.DanLisaId;
            target.DiscordId = source.DiscordId;
            target.Firstname = source.Firstname;
            target.IRacingId = source.IRacingId;
            target.Lastname = source.Lastname;
            target.Team = DefaultGet<TeamDataDTO, TeamEntity>(source.Team);

            return target;
        }

        public TeamEntity MapToTeamEntity(TeamDataDTO source, TeamEntity target = null)
        {
            if (source == null)
                return null;
            if (target == null)
                target = DefaultGet<TeamEntity>(source.Keys);

            if (MapToRevision(source, target) == false)
                return target;

            target.Name = source.Name;
            target.Profile = source.Profile;
            target.TeamColor = source.TeamColor;
            target.TeamHomepage = source.TeamHomepage;
            if (target.Members == null)
                target.Members = new List<LeagueMemberEntity>();
            MapCollection(source.MemberIds
                .Select(x => new LeagueMemberInfoDTO() { MemberId = x }), target.Members, GetMemberEntity, x => x.Keys, 
                removeFromCollection: true);

            return target;
        }
    }
}
