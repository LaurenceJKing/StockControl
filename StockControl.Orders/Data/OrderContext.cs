using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StockControl.Orders.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
    }

    public interface IOrderContext
    {
        DbSet<DraftOrder> DraftOrders { get; }
        IQueryable<SupplierOrder> SupplierOrders { get; }
        IQueryable<LineItem> LineItems { get; }
        IQueryable<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
