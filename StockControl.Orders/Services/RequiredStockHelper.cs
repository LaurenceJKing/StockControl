using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders.Services
{
    public static class RequiredStockHelper
    {
        public static IEnumerable<(LineItem LineItem, Quantity Quantity)>
            SelectItemsToOrder(
            this IEnumerable<StockRequirement> requiredStock,
            IEnumerable<LineItem> lineItems) =>
            requiredStock
                .Select(_ => (
                    LineItem: _.Product.SelectLineItemToOrder(lineItems),
                    QuantityToOrder: _.QuantityRequired
                ))
                .GroupBy(_ =>
                    _.LineItem.Id)
                .Select(_ =>
                    (_.First().LineItem,
                    Quantity: (Quantity)_.Sum(item => item.TotalPacks())))
                .Where(_ =>
                    _.Quantity >=
                    _.LineItem.Product.OrderingStrategy.MinimumOrderQuantity);

        private static Quantity TotalPacks(
            this (LineItem LineItem, Quantity Quantity) _) =>
            _.LineItem.CalculateTotalPacks(_.Quantity);
    }
}
