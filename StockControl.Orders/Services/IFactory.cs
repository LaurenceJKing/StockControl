using System.Threading;
using System.Threading.Tasks;

namespace StockControl.Orders.Services
{
    public interface IFactory<TSeed, T>
    {
        public Task<T> Create(
            TSeed seed,
            CancellationToken cancellationToken = default);
    }
}
