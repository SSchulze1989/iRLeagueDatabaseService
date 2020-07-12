using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Filters
{
    public static class LeagueRoles
    {
        public const string Admin = "Administrator";
        public const string User = "User";
        public const string UserOrAdmin = Admin + "," + User;
    }
}