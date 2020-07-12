using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace iRLeagueRESTService.Data
{
    public class CustomUserStore : UserStore<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUserStore<IdentityUser>, IUserStore<IdentityUser, string>, IDisposable
    {
        public CustomUserStore(DbContext context) : base(context)
        {
        }
    }
}