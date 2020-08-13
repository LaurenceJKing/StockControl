using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders
{
    public class Delivery
    {
        public Delivery(int supplierId, IEnumerable<DeliveryItem> items)
        {
            SupplierId = supplierId;
            Items = items.Where(item => item.LineItem.SupplierId == supplierId);
        }

        public int SupplierId { get; }
        public IEnumerable<DeliveryItem> Items { get; }
    }

    public class DeliveryItem
    {
        public LineItem LineItem { get; }

        public Quantity Quantity { get; }

        public DeliveryItem(LineItem lineItem, Quantity quantity)
        {
            LineItem = lineItem;
            Quantity = quantity;
        }
    }
}