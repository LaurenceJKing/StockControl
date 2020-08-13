using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StockControl.Orders.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FluentValidation;

namespace StockControl.Orders.Application
{
    public class AddOrUpdateDraftOrderHandler : IRequestHandler<AddOrUpdateDraftOrderRequest, DraftOrderItemViewModel?>
    {
        private readonly IMapper _mapper;
        private readonly OrderContext _context;

        public AddOrUpdateDraftOrderHandler(OrderContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DraftOrderItemViewModel?> Handle(
            AddOrUpdateDraftOrderRequest request,
            CancellationToken cancellationToken)
        {
            var lineItem = await _context.LineItems.SingleOrDefaultAsync(
                _ => _.Id == request.LineItemId,
                cancellationToken);

            var order = await _context.DraftOrders.Include(o => o.Items.Select(i => i.LineItem)).SingleOrDefaultAsync(
                _ => _.Id == request.OrderId,
                cancellationToken);

            if (lineItem is null || order is null) return null;

            order.AddOrUpdate(lineItem, (Quantity)request.Quantity);

            _context.Update(order);
            await _context.SaveChangesAsync();

            return _mapper.Map<DraftOrderItemViewModel>(order.Items.SingleOrDefault(
                _ => _.LineItem.Id == request.LineItemId));
        }
    }

    public class AddOrUpdateDraftOrderRequest: IRequest<DraftOrderItemViewModel?>
    {
        public int OrderId { get; set; }
        public int LineItemId { get; set; }
        public int Quantity { get; set; }
    }
}
