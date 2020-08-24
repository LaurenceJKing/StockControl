using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace StockControl.Orders.Data
{
    public class OrderContext: DbContext, IOrderContext
    {
        public IQueryable<SupplierOrder> SupplierOrders => Set<SupplierOrder>();

        public IQueryable<LineItem> LineItems => Set<LineItem>();

        public IQueryable<Product> Products => Set<Product>();

        public DbSet<DraftOrder> DraftOrders => Set<DraftOrder>();


        public OrderContext(
            DbContextOptions<OrderContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }

    public interface IOrderContext
    {
        DbSet<DraftOrder> DraftOrders { get; }
        IQueryable<SupplierOrder> SupplierOrders { get; }
        IQueryable<Product> Products { get; }
        IQueryable<LineItem> LineItems { get; }
        public EntityEntry<T> Update<T>(T entity) where T : class;
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}