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
using iRLeagueDatabase.DataTransfer.User;
using iRLeagueUserDatabase;

namespace iRLeagueRESTService.Controllers
{
    public class UserController : ApiController
    {
        [HttpGet]
        [ActionName("GetUser")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult GetUser([FromUri] string id)
        {
            if (id == null || id == "")
                return NotFound();

            UserDTO userDto = new UserDTO();
            using (var client = new UsersDbContext())
            {
                IdentityUser user = null;
                user = client.Users.Find(id);

                if (user == null)
                    user = client.Users.SingleOrDefault(x => x.UserName == id);
                if (user == null)
                    return NotFound();

                userDto.UserId = user.Id;
                userDto.UserName = user.UserName;

                var userProfile = client.UserProfiles.Find(user.Id);
                if (userProfile != null)
                {
                    userDto.Firstname = userProfile.Firstname;
                    userDto.Lastname = userProfile.Lastname;
                    userDto.MemberId = userProfile.MemberId;
                }
            }

            return Ok(userDto);
        }

        [HttpGet]
        [ActionName("Authenticate")]
        //[Route("User")]
        //[Route("User/Authenticate")]
        [IdentityBasicAuthentication]
        [Authorize]
        public IHttpActionResult Authenticate()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(true);
            }

            return BadRequest("Authentication failed");
        }

        //[HttpGet]
        //[ActionName("ListAll")]
        //[Route("User/ListAll")]
        //[IdentityBasicAuthentication]
        //[Authorize(Roles = LeagueRoles.Admin)]
        //public IHttpActionResult ListAllUsers()
        //{
        //    List<UserProfileDTO> users = new List<UserProfileDTO>();

        //    using ( var client = new UsersDbContext())
        //    {
        //        var aspNetUsers = client.Users.ToList();
        //        var userProfiles = client.UserProfiles.ToList();

        //        foreach(var aspNetUser in aspNetUsers)
        //        {
        //            var user = new UserProfileDTO()
        //            {
        //                UserId = aspNetUser.Id,
        //                UserName = aspNetUser.UserName,
        //                Email = aspNetUser.Email
        //            };

        //            var userProfile = userProfiles.SingleOrDefault(x => x.Id == aspNetUser.Id);
        //            if (userProfile != null)
        //            {
        //                user.Firstname = userProfile.Firstname;
        //                user.Lastname = userProfile.Lastname;
        //                user.ProfileText = userProfile.ProfileText;
        //            }
        //            users.Add(user);
        //        }
        //    }

        //    return Ok(users);
        //}

        [HttpPost]
        [ActionName("CreateUser")]
        [IdentityBasicAuthentication]
        //[Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult CreateUser([FromBody] AddUserDTO userDto)
        {
            var userName = userDto.UserName;
            var password = userDto.Password;
            IdentityUser user;
            UserProfile userProfile = new UserProfile();

            using (UserManager<IdentityUser> userManager = CreateUserManager())
            {
                user = userManager.FindByName(userName);
                if (user == null)
                {
                    var result = userManager.Create(new IdentityUser(userName), password);
                    if (result.Succeeded)
                    {
                        user = userManager.Find(userName, password);
                        userManager.AddToRole(user.Id, "User");

                        if (userDto != null)
                        {
                            userManager.SetEmailAsync(user.Id, userDto.Email);
                        }

                        using (var context = new UsersDbContext())
                        {
                            var profileUser = context.Users.Find(user.Id);
                            userProfile = context.UserProfiles.Find(user.Id);
                            if (userProfile == null)
                            {
                                userProfile = new UserProfile()
                                {
                                    User = profileUser
                                };
                                context.UserProfiles.Add(userProfile);
                            }

                            if (userDto != null)
                            {
                                userProfile.Firstname = userDto.Firstname;
                                userProfile.Lastname = userDto.Lastname;
                                userProfile.MemberId = userDto.MemberId;
                                userProfile.ProfileText = userDto.ProfileText;
                            }
                            context.SaveChanges();
                        }
                    }
                }
                else
                {
                    return Conflict();
                }
            }

            var userModel = new UserProfileDTO() 
            { 
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Firstname = userProfile.Firstname,
                Lastname = userProfile.Lastname,
                MemberId = userProfile.MemberId
            };

            return Json(userModel);
        }

        [HttpPut]
        [IdentityBasicAuthentication]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public IHttpActionResult PutUser([FromBody] UserProfileDTO userDto)
        {
            if (userDto == null)
                return BadRequest("Content was null");

            using(var client = new UsersDbContext())
            {

            }

            return null;
        }

        [HttpGet]
        [ActionName("AddRole")]
        [IdentityBasicAuthentication]
        [Authorize(Roles = "Administrator")]
        public IHttpActionResult AddRole(string roleName)
        {
            using (RoleManager<IdentityRole> roleManager = CreateRoleManager())
            {

                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    var result = roleManager.Create(new IdentityRole(roleName));
                    if (result.Succeeded)
                        role = roleManager.FindByName(roleName);
                }

                return Ok(role.Id);
            }
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
            using (var userManager = CreateUserManager())
            {
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

                return BadRequest("Could not add role to user:\n" + result.Errors.Aggregate((x, y) => x + "\n" + y));
            }
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