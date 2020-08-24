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
using Mediatr.Pipelines.Caching;

namespace StockControl.Orders.Application
{
    public static class AddOrUpdateDraftOrderItem
    {
        public class Request: IIdempotentRequest<DraftOrderItemViewModel>
        {
            public int OrderId { get; set; }
            public int LineItemId { get; set; }
            public int Quantity { get; set; }
            public string IdempotencyKey { get; set; } = "";
        }

        public class Handler : IRequestHandler<Request, DraftOrderItemViewModel>
        {
            private readonly IMapper _mapper;
            private readonly OrderContext _context;

            public Handler(OrderContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<DraftOrderItemViewModel> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var lineItem = await _context.LineItems
                    .SingleAsync(_ => _.Id == request.LineItemId, cancellationToken);

                var order = await _context.DraftOrders
                    .Include(o => o.Items).ThenInclude(o => o.LineItem)
                    .SingleAsync(_ => _.Id == request.OrderId, cancellationToken);

                order.AddOrUpdate(lineItem, (Quantity)request.Quantity);

                _context.Update(order);
                await _context.SaveChangesAsync();

                return _mapper.Map<DraftOrderItemViewModel>(order.Items.SingleOrDefault(
                    _ => _.LineItem.Id == request.LineItemId));
            }
        }

        public class Validator: AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.Quantity).GreaterThan(1);
            }
        }

    }
}
