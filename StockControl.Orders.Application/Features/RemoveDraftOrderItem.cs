using AutoMapper;
using Mediatr.Pipelines.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockControl.Orders.Application
{
    public static class RemoveDraftOrderItem
    {
        public class Request : IIdempotentRequest<Unit>
        {
            public int LineItemId { get; set; }
            public int OrderId { get; set; }

            public string IdempotencyKey { get; set; } = "";
        }

        public class Handler :
        IRequestHandler<Request, Unit>
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
                var lineItem = await context.LineItems
                    .SingleAsync(
                        _ => _.Id == request.LineItemId,
                        cancellationToken);

                var order = await context.DraftOrders
                    .Include(o => o.Items)
                    .ThenInclude(o => o.LineItem)
                    .SingleAsync(
                        _ => _.Id == request.OrderId,
                        cancellationToken);

                order.Remove(lineItem);

                context.Update(order);
                await context.SaveChangesAsync();

                return default;
            }
        }
    }
}
