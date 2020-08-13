using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StockControl.Orders.Application;

namespace StockControl.Controllers
{
    public class DraftOrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("[controller]")]
        public IActionResult Create(
            [FromServices] IMediator mediator,
            IEnumerable<StockRequirementDTO> stockRequirements)
        {
            var order = mediator.Send(stockRequirements);
            return Json(order);
        }

        //[HttpPut("[controller]/{id}/[action]")]
        //public IActionResult Add(
        //    int id,
        //    int lineItemId,
        //    int quantity)
        //{

        //}
    }
}
