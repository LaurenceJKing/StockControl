using AutoMapper;
using Mediatr.Pipelines.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using StockControl.Orders.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockControl.Orders.Application
{
    public static class PlaceDraftOrder
    {
        public class Request : IIdempotentRequest<Unit>
        {
            public int OrderId { get; set; }
            public string IdempotencyKey { get; set; } = "";
        }

        public class Handler : IRequestHandler<Request, Unit>
        {
            private readonly OrderContext context;

            public Handler(OrderContext context)
            {
                this.context = context;
            }

            public async Task<Unit> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var order = await context.DraftOrders
                    .Include(o => o.Items).ThenInclude(l => l.LineItem.Product)
                    .SingleAsync(o => o.Id == request.OrderId, cancellationToken);

                order.Place();

                context.Update(order);
                await context.SaveChangesAsync(cancellationToken);

                var supplierOrders = order.Events
                    .OfType<OrderPlacedEvent>()
                    .Select(e => e.Order);

                return default;
            }
        }
    }
}
