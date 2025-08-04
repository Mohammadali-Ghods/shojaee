using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using System.Security.Claims;
using Shojaee.ProductModule;
using Shojaee.UserModule;

namespace Shojaee.CommentModule
{
    public class CommentController : ControllerBase
    {
        [HttpGet("GetProductComments")]
        public async Task<List<Comment>> GetProductComments([FromQuery] string productId)
        {
            var result = await DB.Find<Comment>().Match(x => x.ProductID == productId).ExecuteAsync();

            for (int i = 0; i <= result.Count - 1; i++)
            {
                result[i].User = await DB.Find<User>().Match(x => x.ID == result[i].UserID).ExecuteFirstAsync();
                result[i].User.LastToken = "";
                result[i].User.Password = "";
                result[i].User.Role = "";
            }

            return result;
        }

        [HttpGet("GetAllComments")]
        [Authorize(Roles = "Admin")]
        public async Task<List<Comment>> GetAllComments()
        {
            var result = await DB.Find<Comment>().ExecuteAsync();

            for (int i = 0; i <= result.Count - 1; i++)
            {
                result[i].User = await DB.Find<User>().Match(x => x.ID == result[i].UserID).ExecuteFirstAsync();
                result[i].User.LastToken = "";
                result[i].User.Password = "";
                result[i].User.Role = "";

                var product = await DB.Find<Product>().Match(x => x.ID == result[i].ProductID).ExecuteFirstAsync();
                if (product == null) continue;
                result[i].ProductName = product.ProductName;
            }

            return result;
        }

        [HttpPost("AddComment")]
        [Authorize]
        public async Task<Comment> AddComment([FromBody] Comment comment)
        {
            comment.CreatedDate = DateTime.Now;
            comment.UserID = User.Identity.Name;

            await DB.InsertAsync<Comment>(comment);

            return comment;
        }

        [HttpPost("EditComment")]
        [Authorize]
        public async Task<Comment> EditComment([FromBody] Comment comment)
        {
            var currentComment = await DB.Find<Comment>().Match(x => x.ID == comment.ID).ExecuteFirstAsync();

            if (currentComment == null) return null;
            if (currentComment.UserID != User.Identity.Name && GetRoleFromJWTToken() != "Admin")
                return null;

            currentComment.CommentContent = comment.CommentContent;

            await DB.Update<Comment>()
            .MatchID(currentComment.ID)
         .ModifyExcept(x => new { x.ID }, currentComment)
         .ExecuteAsync();

            return comment;
        }

        [HttpPost("RemoveComment")]
        [Authorize]
        public async Task TaskRemoveComment([FromBody] Comment comment)
        {
            var currentComment = await DB.Find<Comment>().Match(x => x.ID == comment.ID).ExecuteFirstAsync();

            if (currentComment == null) return;
            if (currentComment.UserID != User.Identity.Name && GetRoleFromJWTToken() != "Admin")
                return;

            await DB.DeleteAsync<Comment>(comment.ID);
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
