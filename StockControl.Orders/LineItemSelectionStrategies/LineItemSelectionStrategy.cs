using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders.LineItemSelectionStrategies
{
    public class LineItemSelectionStrategy
    {
        internal virtual LineItem SelectLineItemToOrder(
            IEnumerable<LineItem> lineItems,
            Product product) =>
            lineItems
            .Where(lineItem => lineItem.Product.Id == product.Id)
            .OrderBy(lineItem => lineItem.PriceInPounds)
            .First();
    }
}
