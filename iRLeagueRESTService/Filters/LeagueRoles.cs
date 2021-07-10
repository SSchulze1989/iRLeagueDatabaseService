using iRLeagueDatabase.Enums;
using iRLeagueDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Filters
{
    public static class LeagueRoles
    {
        private static ReadOnlyDictionary<LeagueRoleEnum, string> RoleNames { get; }

        static LeagueRoles()
        {
            Dictionary<LeagueRoleEnum, string> roleNames = new Dictionary<LeagueRoleEnum, string>();

            // Get rolenames from enum
            foreach (var role in Enum.GetValues(typeof(LeagueRoleEnum)).OfType<LeagueRoleEnum>())
            {
                var roleName = role.GetAttribute<LeagueRoleNameAttribute>()?.RoleName;
                if (string.IsNullOrEmpty(roleName))
                {
                    roleName = Enum.GetName(typeof(LeagueRoleEnum), role);
                }
                roleNames.Add(role, roleName);
            }

            RoleNames = new ReadOnlyDictionary<LeagueRoleEnum, string>(roleNames);
        }

        public static string GetRoleName(LeagueRoleEnum role)
        {
            return role.GetAttribute<LeagueRoleNameAttribute>()?.RoleName ?? Enum.GetName(typeof(LeagueRoleEnum), role);
        }

        public static IDictionary<LeagueRoleEnum, string> GetAvailableRoles()
        {
            return new ReadOnlyDictionary<LeagueRoleEnum, string>(RoleNames);
        }
    }
}