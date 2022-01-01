using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockControl.Orders;

namespace StockControl.Api.Orders
{
    [Route("[controller]")]
    [ApiController]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class OrdersController : ControllerBase

    {
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromServices]IOrderRepository orderRepository)
        {
            var id = "dc069aeb-5eb9-4f1b-9dba-bcc7d7b0c47d";

            await orderRepository.Create(new Order());

            return Created($"orders/{id}", null);            
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
