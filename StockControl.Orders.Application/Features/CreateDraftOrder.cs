using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Mediatr.Pipelines.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using StockControl.Orders.Services;

namespace StockControl.Orders.Application
{
    public static class CreateDraftOrder
    {
        public class Request : IIdempotentRequest<DraftOrderViewModel>
        {
            public IEnumerable<StockLevelSummary> RequiredStock { get; set; } = new StockLevelSummary[0];

            public string IdempotencyKey { get; set; } = "";
        }

        public class StockLevelSummary
        {
            public int ProductId { get; set; }

            public int TargetStockLevel { get; set; }

            public int CurrentStockLevel { get; set; }
        }

        public class Handler : IRequestHandler<Request, DraftOrderViewModel>
        {
            public Handler(
                OrderContext context,
                IFactory<IEnumerable<StockRequirement>, DraftOrder> factory,
                IMapper mapper)
            {
                _context = context;
                _factory = factory;
                _mapper = mapper;
            }

            private readonly OrderContext _context;
            private readonly IFactory<IEnumerable<StockRequirement>, DraftOrder> _factory;
            private readonly IMapper _mapper;

            public async Task<DraftOrderViewModel> Handle(
                Request request,
                CancellationToken cancellationToken)
            {
                var requirements = await MapStockRequirements(
                    request.RequiredStock,
                    cancellationToken);

                var order = await _factory.Create(requirements);

                _context.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<DraftOrderViewModel>(order);
            }

            private async Task<IEnumerable<StockRequirement>> MapStockRequirements(
                IEnumerable<StockLevelSummary> stockRequirements,
                CancellationToken cancellationToken)
            {
                var requirements = await Task.WhenAll(
                    stockRequirements.Select(
                        requirement => MapStockRequirement(
                            requirement,
                            cancellationToken)));

                return requirements.Choose();
            }

            private async Task<StockRequirement?> MapStockRequirement(
                StockLevelSummary stockRequirement,
                CancellationToken cancellationToken)
            {
                var product = await _context.Products
                    .Include(p => p.LineItemSelectionStrategy)
                    .SingleOrDefaultAsync(
                        p => p.Id == stockRequirement.ProductId,
                        cancellationToken);

                if (product is null) return null;

                var items = await _context.SupplierOrders
                    .SelectMany(o => o.Items)
                    .Where(item =>
                        item.LineItem.Product.Id == product.Id &&
                        item.QuantityAwaitingDelivery > 0)
                    .ToListAsync();

                var quantityAwaitingDelivery = items.Sum(item =>
                    item.LineItem.CalculateTotalItems(item.QuantityAwaitingDelivery));

                return new StockRequirement(
                    product,
                    (Quantity)stockRequirement.TargetStockLevel,
                    (Quantity)stockRequirement.CurrentStockLevel,
                    (Quantity)quantityAwaitingDelivery);
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleForEach(r => r.RequiredStock)
                    .ChildRules(_ =>
                    {
                        _.RuleFor(s => s.TargetStockLevel).GreaterThanOrEqualTo(0);
                        _.RuleFor(s => s.CurrentStockLevel).GreaterThanOrEqualTo(0);
                    });
            }
        }
    }
}
