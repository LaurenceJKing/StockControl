using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using StockControl.Orders.Services;

namespace StockControl.Orders.Application
{
    public static class IEnumarableHelpers
    {
        public static IEnumerable<T> Choose<T>(this IEnumerable<T?> source) where T:class
        {
            return source.Where(_ => _ != null)!;
        }

        public static IEnumerable<T> Choose<T>(this IEnumerable<T?> source) where T : struct
        {
            return source.Where(_ => _.HasValue).Select(_ => _.Value);
        }
    }


    public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, DraftOrderViewModel>
    {
        public CreateOrderHandler(
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
            CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            var requirements = await MapStockRequirements(
                request.RequiredStock,
                cancellationToken);

            var order = await _factory.Create(requirements);

            _context.Add(order);
            await  _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DraftOrderViewModel>(order);
        }

        private async Task<IEnumerable<StockRequirement>> MapStockRequirements(
            IEnumerable<StockRequirementDTO> stockRequirements,
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
            StockRequirementDTO stockRequirement,
            CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.LineItemSelectionStrategy)
                .SingleOrDefaultAsync(
                    p => p.Id == stockRequirement.ProductId,
                    cancellationToken);

            if (product is null) return null;

            var quantityAwaitingDelivery = await _context.SupplierOrders
                .SelectMany(o => o.Items)
                .Where(item =>
                    item.LineItem.Product.Id == product.Id &&
                    item.QuantityAwaitingDelivery > 0)
                .SumAsync(item =>
                    item.LineItem.CalculateTotalItems(item.QuantityAwaitingDelivery));

            return new StockRequirement(
                product,
                (Quantity)stockRequirement.TargetStockLevel,
                (Quantity)stockRequirement.CurrentStockLevel,
                (Quantity)quantityAwaitingDelivery);
        }
    }

    public class CreateOrderRequest: IRequest<DraftOrderViewModel>
    {
        public IEnumerable<StockRequirementDTO> RequiredStock { get; }
    }

    public class StockRequirementDTO
    {
        public int ProductId { get; }

        public int TargetStockLevel { get; }

        public int CurrentStockLevel { get; }
    }

    public class DraftOrderViewModel
    {
        public int Id { get; set; }
        public IEnumerable<DraftOrderItemViewModel> Items { get; set; }
    }

    public class DraftOrderItemViewModel {
        public int LineItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

    public class DraftOrderProfile: Profile
    {
        public DraftOrderProfile()
        {
            CreateMap<DraftOrder, DraftOrderViewModel>();
            CreateMap<DraftOrderItem, DraftOrderItemViewModel>()
                .ForMember(
                item => item.ProductName,
                opt => opt.MapFrom(
                    src => src.LineItem.Product.Name));
        }
    }
}
