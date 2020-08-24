using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders.LineItemSelectionStrategies
{
    public class LineItemSelectionStrategy
    {
        public int Id { get; private set; }

        internal virtual LineItem SelectLineItemToOrder(
            IEnumerable<LineItem> lineItems,
            Product product) =>
            lineItems
            .Where(lineItem => lineItem.Product.Id == product.Id)
            .OrderBy(lineItem => lineItem.PriceInPounds)
            .First();
    }
}
