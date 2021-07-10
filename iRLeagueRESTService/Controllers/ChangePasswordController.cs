using iRLeagueDatabase.DataTransfer.User;
using iRLeagueRESTService.Filters;
using iRLeagueRESTService.Results;
using iRLeagueUserDatabase;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace iRLeagueRESTService.Controllers
{
    public class ChangePasswordController : ApiController
    {

        // POST api/<controller>
        [IdentityBasicAuthentication]
        [LeagueAuthorize(Roles = iRLeagueDatabase.Enums.LeagueRoleEnum.User)]
        public IHttpActionResult Post([FromBody] AddUserDTO userDto)
        {
            if (userDto == null)
            {
                return BadRequest("Content was null");
            }

            if (userDto.UserId != User.Identity.GetUserId() && User.IsInRole("Administrator") == false)
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            using (var userManager = CreateUserManager())
            {
                userManager.UserTokenProvider = new TotpSecurityStampBasedTokenProvider<IdentityUser, string>();
                var resetToken = userManager.GeneratePasswordResetToken(userDto.UserId);
                userManager.ResetPassword(userDto.UserId, resetToken, userDto.Password);
            }

            return Ok();
        }

        private static UserManager<IdentityUser> CreateUserManager()
        {
            return new UserManager<IdentityUser>(new UserStore<IdentityUser>(new UsersDbContext()));
        }
    }
}