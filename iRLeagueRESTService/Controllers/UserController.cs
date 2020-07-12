using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using iRLeagueDatabase.DataTransfer.Members;

namespace iRLeagueRESTService.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [ActionName("CreateUser")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult CreateUser(string userName, string password)
        {
            UserManager<IdentityUser> userManager = CreateUserManager();

            var user = userManager.Find(userName, password);
            if (user == null)
            {
                var result = userManager.Create(new IdentityUser(userName), password);
                if (result.Succeeded)
                    user = userManager.Find(userName, password);
            }

            var userModel = new LeagueUserDTO() { UserName = user.UserName };

            return Json(userModel);
        }

        [HttpGet]
        [ActionName("AddRole")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult AddRole(string roleName)
        {
            RoleManager<IdentityRole> roleManager = CreateRoleManager();

            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                var result = roleManager.Create(new IdentityRole(roleName));
                if (result.Succeeded)
                    role = roleManager.FindByName(roleName);
            }

            return Ok(role.Id);
        }

        [HttpGet]
        [ActionName("AddRole")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult AddRole(string roleName, string leagueName)
        {
            return AddRole(GetRoleName(roleName, leagueName));
        }

        [HttpGet]
        [ActionName("AddUserRole")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult UserAddRole(string userName, string roleName)
        {
            var userManager = CreateUserManager();
            var user = userManager.FindByName(userName);
            if (user == null)
                return BadRequest("Username not found");

            var roleManager = CreateRoleManager();

            var role = roleManager.FindByName(roleName);
            if (role == null)
                return BadRequest("Role " + roleName + " not found");

            var result = userManager.AddToRole(user.Id, role.Name);
            if (result.Succeeded)
                return Ok("Role" + role.Name + " added to " + user.UserName);

            return BadRequest("Could not add role to user:\n" + result.Errors.Aggregate((x,y) => x + "\n" + y));
        }

        [HttpGet]
        [ActionName("AddUserRole")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult UserAddRole(string userName, string roleName, string leagueName)
        {
            return UserAddRole(userName, GetRoleName(roleName, leagueName));
        }

        private static string GetRoleName(string role, string leagueName)
        {
            if (leagueName != null)
                return leagueName + "_" + role;
            else
                return role;
        }

        private static UserManager<IdentityUser> CreateUserManager()
        {
            return new UserManager<IdentityUser>(new UserStore<IdentityUser>(new UsersDbContext()));
        }

        private static RoleManager<IdentityRole> CreateRoleManager()
        {
            return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new UsersDbContext()));
        }
    }
}