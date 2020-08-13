using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders.LineItemSelectionStrategies
{
    public class OrderFromSupplierStrategy : LineItemSelectionStrategy
    {
        public OrderFromSupplierStrategy(int supplierId)
        {
            SupplierId = supplierId;
        }

        public int SupplierId { get; private set; }

        internal override LineItem SelectLineItemToOrder(
            IEnumerable<LineItem> lineItems,
            Product product) =>
            lineItems.First(
                lineItem =>
                lineItem.SupplierId == SupplierId &&
                lineItem.Product.Id == product.Id);
    }
}