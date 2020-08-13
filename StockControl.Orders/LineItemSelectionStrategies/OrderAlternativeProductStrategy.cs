using System.Collections.Generic;

namespace StockControl.Orders.LineItemSelectionStrategies
{
    public class OrderAlternativeProductStrategy : LineItemSelectionStrategy
    {
        public Product AlternativeProduct { get; private set; }

        public OrderAlternativeProductStrategy(Product alternativeProduct)
        {
            AlternativeProduct = alternativeProduct;
        }

        internal override LineItem SelectLineItemToOrder(
            IEnumerable<LineItem> lineItems,
            Product product)
        {
            return base.SelectLineItemToOrder(lineItems, AlternativeProduct);
        }
    }
}