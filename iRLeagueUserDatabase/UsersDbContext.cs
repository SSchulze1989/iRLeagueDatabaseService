using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace iRLeagueUserDatabase
{
    [DbConfigurationType(typeof(iRLeagueDatabase.LeagueDbConfiguration))]
    public class UsersDbContext : IdentityDbContext<IdentityUser>
    {
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        private static string connectionString = $"Data Source={Environment.MachineName}\\IRLEAGUEDB;AttachDbFilename=|DataDirectory|\\iRLeagueManager_UserDatabase.mdf;Initial Catalog=iRLeagueManager_UserDatabase;Integrated Security=True";

        public UsersDbContext() : base(connectionString) { }

        static UsersDbContext()
        {
            Database.SetInitializer(new Initializer());
        }

        private class Initializer : CreateDatabaseIfNotExists<UsersDbContext>
        {
            protected override void Seed(UsersDbContext context)
            {
                IdentityRole role = context.Roles.Add(new IdentityRole("Administrator"));
                context.SaveChanges();

                role = context.Roles.FirstAsync().Result;

                IdentityUser user = new IdentityUser(System.Environment.GetEnvironmentVariable("IRLEAGUE_ADMIN_NAME"));
                user.Roles.Add(new IdentityUserRole { RoleId = role.Id, UserId = user.Id });
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = "hasRegistered",
                    ClaimValue = "true"
                });

                user.PasswordHash = new PasswordHasher().HashPassword(System.Environment.GetEnvironmentVariable("IRLEAGUE_ADMIN_PASSWORD"));
                context.Users.Add(user);
                context.SaveChanges();
                base.Seed(context);
            }
        }
    }
}