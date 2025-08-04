using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using Shojaee.SMSModule;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Shojaee.CommentModule;
using Shojaee.ProductModule;

namespace Shojaee.UserModule
{
    public class UserController : Controller
    {
        private readonly JWTHandlingService _jwtHandlingService;
        private readonly SMSService _smsService;
        public UserController(JWTHandlingService jwtHandlingService,
           SMSService smsService)
        {
            _jwtHandlingService = jwtHandlingService;
            _smsService = smsService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] User input)
        {
            var user = await DB.Find<User>().Match(x => x.UserName == input.UserName
            && x.Password == input.Password).ExecuteFirstAsync();

            if (user == null) { return Unauthorized(); }

            var newToken = _jwtHandlingService.JwtGenerator(user.ID, user.Role, 24);
            user.LastToken = newToken;
            await DB.Update<User>()
            .MatchID(user.ID)
            .ModifyExcept(x => new { x.ID }, user)
            .ExecuteAsync();

            return Ok(user);
        }

        [HttpPost("OneTimePassword")]
        public async Task<ActionResult> OneTimePassword([FromBody] User input)
        {
            var user = await DB.Find<User>().Match(x => x.UserName == input.UserName).ExecuteFirstAsync();

            if (user == null)
            {
                user = new User()
                {
                    ID = Guid.NewGuid().ToString(),
                    Role = "User",
                    UserName = input.UserName
                };

                await DB.InsertAsync<User>(user);
            }

            user.Password = new Random().Next(11111, 99999).ToString();

            await _smsService.SendOneTimePassword(user.Password, user.UserName);

            await DB.Update<User>()
         .MatchID(user.ID)
         .ModifyExcept(x => new { x.ID }, user)
         .ExecuteAsync();

            return Ok();
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<List<User>> GetAllUsers()
        {
            return await DB.Find<User>().ExecuteAsync();
        }

        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
        public async Task<User> AddUser([FromBody] User user)
        {
            user.CreatedDate = DateTime.Now;
            await DB.InsertAsync<User>(user);
            return user;
        }

        [HttpPost("EditUser")]
        [Authorize]
        public async Task<User> EditUser([FromBody] User user)
        {
            var currentuser = await DB.Find<User>().Match(x => x.ID == user.ID).ExecuteFirstAsync();

            if (currentuser == null) return null;
            if (currentuser.ID != User.Identity.Name && GetRoleFromJWTToken() != "Admin")
                return null;

            currentuser.ImageAddress = user.ImageAddress;
            currentuser.FullName = user.FullName;
            currentuser.Disabled = user.Disabled;
            currentuser.Role = user.Role;

            await DB.Update<User>()
            .MatchID(currentuser.ID)
         .ModifyExcept(x => new { x.ID }, currentuser)
         .ExecuteAsync();

            return currentuser;
        }

        [HttpPost("RemoveUser")]
        [Authorize]
        public async Task RemoveUser([FromBody] User user)
        {
            var currentuser = await DB.Find<User>().Match(x => x.ID == user.ID).ExecuteFirstAsync();

            if (currentuser == null) return;
            if (currentuser.ID != User.Identity.Name && GetRoleFromJWTToken() != "Admin")
                return;

            await DB.DeleteAsync<User>(user.ID);
        }

        protected string GetRoleFromJWTToken()
        {
            string result = "";
            if (base.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                result = (from x in claimsIdentity.Claims.ToList()
                          where x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                          select x.Value).FirstOrDefault();
            }

            return result;
        }
    }
}
