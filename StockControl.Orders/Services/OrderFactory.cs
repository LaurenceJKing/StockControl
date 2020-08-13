using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StockControl.Orders.Services
{
    public class OrderFactory : IFactory<IEnumerable<StockRequirement>, DraftOrder>
    {
        private readonly OrderContext _data;

        public OrderFactory(OrderContext data)
        {
            _data = data;
        }

        public async Task<DraftOrder> Create(
            IEnumerable<StockRequirement> requiredStock,
            CancellationToken cancellationToken = default)
        {
            var lineItems = await _data.LineItems
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

            var itemsToOrder = requiredStock
                .SelectItemsToOrder(lineItems);

            var order = new DraftOrder();

            foreach (var item in itemsToOrder)
                order.AddOrUpdate(item.LineItem, item.Quantity);

            return order;
        }
    }
}
