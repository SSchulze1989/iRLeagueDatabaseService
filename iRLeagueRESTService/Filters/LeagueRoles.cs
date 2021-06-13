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
        private static Dictionary<LeagueRoleEnum, string> RoleNames { get; }

        static LeagueRoles()
        {
            // Get rolenames from enum
            foreach (var role in Enum.GetValues(typeof(LeagueRoleEnum)).OfType<LeagueRoleEnum>())
            {
                var roleName = role.GetAttribute<LeagueRoleNameAttribute>()?.RoleName;
                if (string.IsNullOrEmpty(roleName))
                {
                    roleName = Enum.GetName(typeof(LeagueRoleEnum), role);
                }
                RoleNames.Add(role, roleName);
            }
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