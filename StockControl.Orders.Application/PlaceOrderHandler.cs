using AutoMapper;
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
    public class PlaceOrder
    {
        public class Handler : IRequestHandler<Request, SupplierOrderViewModel[]>
        {
            private readonly OrderContext context;
            private readonly IMapper mapper;

            public Handler(OrderContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<SupplierOrderViewModel[]> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var order = await context.DraftOrders
                    .Include(o => o.Items.Select(l => l.LineItem.Product))
                    .SingleOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

                order.Place();

                context.Update(order);
                await context.SaveChangesAsync(cancellationToken);

                var supplierOrders = order.Events
                    .OfType<OrderPlacedEvent>()
                    .Select(e => e.Order);

                return mapper.Map<SupplierOrderViewModel[]>(supplierOrders);
            }
        }

        public class Request : IRequest<SupplierOrderViewModel[]>
        {
            public int OrderId { get; internal set; }
        }
    }

    public class SupplierOrderViewModel
    {

    }
}
