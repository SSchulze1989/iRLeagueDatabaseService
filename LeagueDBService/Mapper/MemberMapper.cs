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

            return target;
        }
    }

    public partial class EntityMapper
    {
        private void RegisterMemberTypeMaps()
        {
            RegisterTypeMap<LeagueMemberDataDTO, LeagueMemberEntity>(MapToMemberEntity);
        }
        public LeagueMemberEntity GetMemberEntity(LeagueMemberInfoDTO source)
        {
            if (source == null)
                return null;

            LeagueMemberEntity target;

            if (source.MemberId == null)
                target = new LeagueMemberEntity();
            else
                target = DbContext.Set<LeagueMemberEntity>().Find(source.MemberId);

            if (target == null)
                throw new EntityNotFoundException(nameof(LeagueMemberEntity), "Could not find Entity in Database.", source.MemberId);

            return target;
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

            return target;
        }
    }
}
