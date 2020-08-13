using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using StockControl.Orders;
using StockControl.Orders.LineItemSelectionStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Xunit;

namespace TestsFor.StockControl.Orders
{
    public class Product_Tests
    {
        [Property(Arbitrary = new[] { typeof(LineItemGenerator)})]
        public Property SelectLineItemToOrder_should_return_cheapest_item_given_default_LineItemSelectionStrategy(
            List<LineItem> lineItems)
        {
            Action test = () =>
            {
                var product = lineItems[0].Product;
                var lineItem = product.SelectLineItemToOrder(lineItems);

                lineItem.PriceInPounds.Should().Be(
                    lineItems
                    .Where(l => l.Product.Id == product.Id)
                    .Min(l => l.PriceInPounds));
            };

            return test.When(lineItems.Count > 0);
        }

        [Property(Arbitrary = new[] { typeof(LineItemGenerator) })]
        public Property SelectLineItemToOrder_should_return_cheapest_alternative_item_given_OrderAlternativeProductStrategy(
           List<LineItem> lineItems)
        {
            Action test = () =>
            {
                var product = lineItems[0].Product;
                product.LineItemSelectionStrategy =
                new OrderAlternativeProductStrategy(lineItems[1].Product);

                var lineItem = product.SelectLineItemToOrder(lineItems);

                lineItem.PriceInPounds.Should().Be(
                    lineItems
                    .Where(l => l.Product.Id == lineItems[1].Product.Id)
                    .Min(l => l.PriceInPounds));
            };

            return test.When(lineItems.Count > 1);
        }

        [Property(Arbitrary = new[] { typeof(LineItemGenerator) })]
        public Property SelectLineItemToOrder_should_return_specific_item_given_OrderFromSupplierStrategy(
           List<LineItem> lineItems)
        {
            Action test = () =>
            {
                var product = lineItems[0].Product;
                product.LineItemSelectionStrategy =
                new OrderFromSupplierStrategy(lineItems[0].SupplierId);

                var lineItem = product.SelectLineItemToOrder(lineItems);

                lineItem.SupplierId.Should().Be(lineItems[0].SupplierId);
            };

            return test.When(lineItems.Count > 0);
        }
    }
}
