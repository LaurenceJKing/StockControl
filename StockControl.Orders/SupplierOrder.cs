using System;
using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders
{
    public class SupplierOrder
    {
        private SupplierOrder() { }
        public SupplierOrder(
            int supplierId,
            int orderId,
            IEnumerable<DraftOrderItem> items)
        {
            SupplierId = supplierId;
            OrderId = orderId;
            _items = items
                .Where(item => item.LineItem.SupplierId == supplierId)
                .Select(item => new SupplierOrderItem(
                    item.LineItem,
                    item.Quantity))
                .ToList();
        }

        private readonly List<SupplierOrderItem> _items = new List<SupplierOrderItem>();

        public int SupplierId { get; private set; }
        public int Id { get; private set; }
        public int OrderId { get; private set; }
        public IEnumerable<SupplierOrderItem> Items => _items;

        public void ProcessDelivery(Delivery delivery)
        {
            if (delivery.SupplierId != SupplierId)
                throw new InvalidOperationException();

            foreach(var item in delivery.Items)
            {
                var orderItem = Items.FirstOrDefault(
                    i => i.LineItem.Id == item.LineItem.Id);

                if (orderItem is null) continue;

                orderItem.AddDelivery(item.Quantity);
            }
        }
    }
}
