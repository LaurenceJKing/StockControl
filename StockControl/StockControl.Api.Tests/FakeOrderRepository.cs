using StockControl.Orders;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StockControl.Api.Tests
{
    internal class FakeOrderRepository: Collection<Order>, IOrderRepository
    {
        public FakeOrderRepository()
        {
        }

        public Task Create(Order newOrder)
        {
            Add(newOrder);
            return Task.CompletedTask;
        }
    }
}