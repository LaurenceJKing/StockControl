using AutoMapper;
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
    public class RemoveItemFromDraftOrderHandler :
        IRequestHandler<RemoveItemFromDraftOrderRequest, DraftOrderItemViewModel>
    {
        private readonly OrderContext context;
        private readonly IMapper mapper;
        public async Task<DraftOrderItemViewModel> Handle(
            RemoveItemFromDraftOrderRequest request,
            CancellationToken cancellationToken)
        {
            var lineItem = await context.LineItems.SingleOrDefaultAsync(
                _ => _.Id == request.LineItemId,
                cancellationToken);

            var order = await context.DraftOrders.Include(o => o.Items.Select(i => i.LineItem)).SingleOrDefaultAsync(
                _ => _.Id == request.OrderId,
                cancellationToken);

            if (lineItem is null || order is null) return null;

            order.Remove(lineItem);

            context.Update(order);
            await context.SaveChangesAsync();

            return mapper.Map<DraftOrderItemViewModel>(order.Items.SingleOrDefault(
                _ => _.LineItem.Id == request.LineItemId));
        }
    }

    public class RemoveItemFromDraftOrderRequest : IRequest<DraftOrderItemViewModel>
    {
        public int LineItemId { get; internal set; }
        public int OrderId { get; internal set; }
    }
}
