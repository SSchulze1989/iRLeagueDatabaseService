using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using iRLeagueDatabase;

namespace iRLeagueRESTService.Filters
{
    public class LeagueAuthorizeAttribute : AuthorizeAttribute
    {
        private const string LeagueParameterName = "leagueName";

        // rolenames
        private const string adminRoleName = "Administrator";
        private const string userRoleName = "User";
        private const string stewardRoleName = "Steward";
        private const string ownerRoleName = "Owner";

        public new LeagueRoleEnum Roles { get; set; }

        public LeagueAuthorizeAttribute()
        {
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            // return true if no role is required
            if (Roles == LeagueRoleEnum.None)
            {
                return true;
            }

            // get league name from actionContext
            var url = actionContext.Request.RequestUri.AbsoluteUri.Split('?').Last();
            var parameters = GetUrlParameters(url);

            string requestLeagueName = parameters.Get(LeagueParameterName);

            // check for league being set to "public" when only "user" role is required
            // public league will treat everyone as user
            if (Roles == LeagueRoleEnum.User) 
            {
                if (GetLeagueIsPublic(requestLeagueName))
                {
                    return true;
                }
            }

            // get user roles
            var principal = actionContext.RequestContext.Principal;
            if (principal.Identity.IsAuthenticated == false)
            {
                return false;
            }

            // if user is API Administrator
            if (principal.IsInRole("Administrator"))
            {
                return true;
            }

            var checkRoles = GetRolesList(Roles, requestLeagueName);
            foreach (var role in checkRoles)
            {
                if (principal.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }

        private bool GetLeagueIsPublic(string leagueName)
        {
            if (string.IsNullOrEmpty(leagueName))
            {
                return true;
            }

            using (var dbContext = new LeagueDbContext())
            {
                var league = dbContext.Leagues.SingleOrDefault(x => x.LeagueName == leagueName);
                return league?.IsPublic ?? false;
            }
        }

        private IEnumerable<string> GetRolesList(LeagueRoleEnum roles, string leagueName)
        {
            List<string> rolesList = new List<string>();

            if (roles.HasFlag(LeagueRoleEnum.Admin))
            {
                rolesList.Add($"{leagueName}_{adminRoleName}");
            }
            if (roles.HasFlag(LeagueRoleEnum.User))
            {
                rolesList.Add($"{leagueName}_{userRoleName}");
            }
            if (roles.HasFlag(LeagueRoleEnum.Steward))
            {
                rolesList.Add($"{leagueName}_{stewardRoleName}");
            }
            if (roles.HasFlag(LeagueRoleEnum.Owner))
            {
                rolesList.Add($"{leagueName}_{ownerRoleName}");
            }

            return rolesList;
        }

        private NameValueCollection GetUrlParameters(string url)
        {
            var parsed = HttpUtility.ParseQueryString(url);
            return parsed;
        }

        private LeagueRoleEnum GetRolesFromString(string roleString, string leagueName)
        {
            LeagueRoleEnum roleEnum = 0;
            // remove whitespaces
            Regex.Replace(roleString, @"\s+", "");

            var singleRoles = roleString
                .Split(',')
                .Where(x => x.Contains("_"))
                .Select(x => { var parts = x.Split('_'); return new { leagueName = parts[0], role = parts[1] }; })
                .Where(x => x.leagueName == leagueName);

            foreach(var role in singleRoles.Select(x => x.role))
            {
                switch (role)
                {
                    case userRoleName:
                        roleEnum &= LeagueRoleEnum.User;
                        break;
                    case adminRoleName:
                        roleEnum &= LeagueRoleEnum.Admin;
                        break;
                    case stewardRoleName:
                        roleEnum &= LeagueRoleEnum.Steward;
                        break;
                    case ownerRoleName:
                        roleEnum = LeagueRoleEnum.Owner;
                        break;
                    default:
                        break;
                }
            }

            return roleEnum;
        }
    }
}