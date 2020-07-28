using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueUserDatabase
{
    public class UserProfile
    {
        [Key, ForeignKey(nameof(User))]
        public string Id { get; set; }
        public virtual IdentityUser User { get; set; }

        public long? MemberId { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfileText { get; set; }
    }
}