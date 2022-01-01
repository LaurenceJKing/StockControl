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
            var order = new Order();

            await orderRepository.Create(order);

            var response = new CreateOrderResponse
            {
                Id = order.Id.ToString(),
            };

            return Created($"orders/{order.Id}", response);            
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
