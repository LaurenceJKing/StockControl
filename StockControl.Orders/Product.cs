using StockControl.Orders.LineItemSelectionStrategies;
using System.Collections.Generic;

namespace StockControl.Orders
{
    public class Product
    {
        public int Id { get; private set; }

        public string Name { get; set; }

        public LineItemSelectionStrategy LineItemSelectionStrategy { get; set; } = new LineItemSelectionStrategy();

        public OrderingStrategy OrderingStrategy { get; private set; } = new OrderingStrategy();

        public LineItem SelectLineItemToOrder(
            IEnumerable<LineItem> lineItems) =>
            LineItemSelectionStrategy.SelectLineItemToOrder(lineItems, this);
    }
}