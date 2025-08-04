using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using System;

namespace Shojaee.OrderModule
{
    public class OrderController : ControllerBase
    {
        [HttpPost("AddOrEditBasket")]
        [Authorize]
        public async Task<Order> AddOrEditBasket([FromBody] Order order)
        {
            if (order.ID == "0")
            {
                order.UserID = User.Identity.Name;
                order.CreatedDate = DateTime.Now;
                order.ID = Guid.NewGuid().ToString();
                order.IsBasket = true;

                await DB.InsertAsync<Order>(order);
            }
            else
            {
                var _order = await DB.Find<Order>()
                    .Match(x => x.ID == order.ID)
                    .ExecuteFirstAsync();

                await DB.Update<Order>()
                .MatchID(_order.ID)
                .ModifyExcept(x => new { x.ID }, order)
                .ExecuteAsync();
            }

            return order;
        }
    }
}
