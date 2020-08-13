using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using StockControl.Orders;
using StockControl.Orders.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestsFor.StockControl.Orders
{
    public static class SupplierOrder_Tests
    {
        public class ProcessDelivery
        {
            [Property(Arbitrary = new Type[] { typeof(SupplierOrderGenerator) })]
            public Property Should_fail_when_the_delivery_is_from_a_different_supplier(
                SupplierOrder order,
                int supplierId)
            {
                var delivery = GenerateDeliveryForDifferentSupplier(
                    order,
                    supplierId);

                Action test = () =>
                order.Invoking(_ => _.ProcessDelivery(delivery))
                .Should().Throw<InvalidOperationException>();

                return test.When(supplierId != order.SupplierId);          
            }

            [Property(Arbitrary = new Type[] { typeof(SupplierOrderGenerator)})]
            public void Should_update_quantity_delivered_for_each_item(
                SupplierOrder supplierOrder)
            {
                var delivery = GenerateDeliveryForOrder(supplierOrder);
                supplierOrder.ProcessDelivery(delivery);

                foreach(var item in supplierOrder.Items)
                {
                    item.QuantityDelivered.Should()
                        .BeGreaterOrEqualTo((Quantity)0);

                    item.QuantityAwaitingDelivery.Should()
                        .BeGreaterOrEqualTo((Quantity)0)
                        .And
                        .BeLessOrEqualTo(item.QuantityOrdered);
                }
            }

            private Delivery GenerateDeliveryForOrder(SupplierOrder order)
            {
                var rand = new System.Random();
                return new Delivery(
                    order.SupplierId,
                    order.Items.Select(item =>
                    new DeliveryItem(
                        item.LineItem,
                        (Quantity)rand.Next(1, int.MaxValue))));
            }

            private Delivery GenerateDeliveryForDifferentSupplier(
                SupplierOrder order,
                int supplierId)
            {
                var rand = new System.Random();
                return new Delivery(
                    supplierId,
                    order.Items.Select(item =>
                    new DeliveryItem(
                        item.LineItem,
                        (Quantity)rand.Next(1, int.MaxValue))));
            }
        }

        public static class SupplierOrderGenerator
        {
            public static Arbitrary<SupplierOrder> Generate() {
                var rand = new System.Random();
                return LineItemGenerator.GenerateLineItem().Generator.Select(item =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(item, (Quantity)rand.Next(0, int.MaxValue));
                    order.Place();
                    return order.Events.OfType<OrderPlacedEvent>().First().Order;
                }).ToArbitrary();
            }
        }
    }
}
