using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Enums
{
    [Flags]
    public enum LeagueRoleEnum
    {
        None =          0b00000000,
        [LeagueRoleName("User")]
        User =          0b00000001,
        [LeagueRoleName("Steward")]
        Steward =       0b00000011,
        [LeagueRoleName("LightAdmin")]
        LightAdmin =    0b00001100,
        [LeagueRoleName("Adminstrator")]
        Admin =         0b00011111,
        [LeagueRoleName("Owner")]
        Owner =         0b11111111,
    }

    public class LeagueRoleNameAttribute : Attribute
    {
        public string RoleName { get; }

        public LeagueRoleNameAttribute(string roleName)
        {
            RoleName = roleName;
        }
    }
}
