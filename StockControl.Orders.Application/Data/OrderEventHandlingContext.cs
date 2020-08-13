using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using StockControl.Orders.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockControl.Orders.Application.Data
{
    public class OrderEventHandlingContext: IOrderContext
    {
        private readonly OrderContext orderContext;

        private readonly IMediator mediator;

        public OrderEventHandlingContext(
            OrderContext orderContext,
            IMediator mediator)
        {
            this.orderContext = orderContext;
            this.mediator = mediator;
        }

        public DbSet<DraftOrder> DraftOrders => orderContext.DraftOrders;

        public IQueryable<SupplierOrder> SupplierOrders => orderContext.SupplierOrders;

        public IQueryable<LineItem> LineItems => orderContext.LineItems;

        public IQueryable<Product> Products => orderContext.Products;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var events = GetAllNewOrderEvents();
            var totalRecordsSaved = await orderContext.SaveChangesAsync(cancellationToken);
            await PublishAll(events);
            return totalRecordsSaved;
        }

        private IEnumerable<OrderEvent> GetAllNewOrderEvents() =>
            orderContext.ChangeTracker.Entries<OrderEvent>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity);

        private Task PublishAll(IEnumerable<OrderEvent> events) =>
            Task.WhenAll(events
                .Select(e => mediator.Publish(e)));
    }

    public class Notification<T>: INotification where T: class
    {
        public Notification(T body)
        {
            Body = body;
        }

        public T Body { get; }
    }
}
