using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace iRLeagueRESTService.Data
{
    public class CustomIdentityRole : IdentityRole
    {
        public CustomIdentityRole() { }
        public CustomIdentityRole(string roleName, string leagueName) : base(roleName)
        {
            LeagueName = leagueName;
        }

        public string LeagueName { get; set; }
    }
}