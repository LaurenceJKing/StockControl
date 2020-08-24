using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockControl.Orders.Application;

namespace StockControl.Controllers
{
    public class DraftOrdersController : Controller
    {
        [HttpPut("[controller]")]
        public async Task<IActionResult> Create(
            [FromServices] IMediator mediator,
            IEnumerable<CreateDraftOrder.Request> stockRequirements)
        {
            var order = await mediator.Send(stockRequirements);
            return Json(order);
        }

        [HttpPut("[controller]/{id:min(0)}/[action]")]
        public async Task<IActionResult> AddOrUpdate(
            [FromServices] IMediator mediator,
            int id,
            [FromBody] AddOrUpdateDraftOrderItem.Request request)
        {
            request.OrderId = id;
            var orderItem = await mediator.Send(request);
            return Json(orderItem);
        }

        [HttpPut("[controller]/{id:min(1)}/[action]")]
        public async Task<IActionResult> Remove(
            [FromServices] IMediator mediator,
            int id,
            [FromBody] RemoveDraftOrderItem.Request request)
        {
            request.OrderId = id;
            await mediator.Send(request);
            return Ok();
        }

        [HttpPut("[controller]/{id:min(1)}/[action]")]
        public async Task<IActionResult> Place(
            [FromServices] IMediator mediator,
            int id)
        {
            var request = new PlaceDraftOrder.Request
            {
                OrderId = id
            };

            await mediator.Send(request);
            return Ok();
        }

    }
}
