using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using MoreLinq;
using StockControl.Orders;
using StockControl.Orders.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace TestsFor.StockControl.Orders
{
    public static class DraftOrder_Tests
    {
        public static bool IsValidQuantity(this int quantity) => quantity > 0;
        public class AddOrUpdate
        {
            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_fail_when_quantity_less_than_1(
                LineItem lineItem,
                int qty)
            {
                Action test = () => new DraftOrder()
                .Invoking(o => o.AddOrUpdate(lineItem, (Quantity)qty))
                .Should()
                .Throw<ArgumentOutOfRangeException>();

                return test.When(!qty.IsValidQuantity());
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_fail_when_the_order_has_been_placed(
               LineItem lineItem1,
               LineItem lineItem2,
               int qty)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(lineItem1, (Quantity)qty);
                    order.Place();

                    order.Invoking(o => o.AddOrUpdate(lineItem2, (Quantity)qty))
                    .Should().Throw<InvalidOperationException>();
                };
                return test.When(qty.IsValidQuantity());
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_update_when_the_lineItem_has_already_been_added_to_the_order(
               LineItem lineItem,
               int qty1,
               int qty2)
            {
                Action test = () => new DraftOrder().Invoking(o =>
                {
                    o.AddOrUpdate(lineItem, (Quantity)qty1);
                    o.AddOrUpdate(lineItem, (Quantity)qty2);

                    o.Items.Single(o => o.LineItem.Id == lineItem.Id)
                    .Quantity.Should().Be((Quantity)qty2);
                });

                return test.When(qty1.IsValidQuantity() && qty2.IsValidQuantity());
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public void Should_add_lineItems_to_order(
                List<LineItem> items)
            {
                var rand = new System.Random();
                items = items
                    .DistinctBy(lineItem => lineItem.Id)
                    .ToList();
                var order = new DraftOrder();

                foreach (var item in items)
                {
                    order.AddOrUpdate(item, (Quantity)rand.Next(1, int.MaxValue));
                }

                order.Items.Should().HaveCount(items.Count);
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_raise_an_ItemAddedToOrderEvent(
               LineItem lineItem,
               int qty)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(lineItem, (Quantity)qty);

                    order.Events.Last().Should().BeOfType<ItemAddedToOrderEvent>();
                };

                return test.When(qty.IsValidQuantity());
            }
        }
        public class Remove
        {
            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public void Should_fail_when_the_lineItem_has_not_been_added(
            LineItem lineItem)
            {
                new DraftOrder().Invoking(order => order.Remove(lineItem))
                    .Should().Throw<InvalidOperationException>();
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_fail_when_the_order_has_been_placed(
               LineItem lineItem,
               int qty)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(lineItem, (Quantity)qty);
                    order.Place();

                    order.Invoking(o => o.Remove(lineItem))
                    .Should().Throw<InvalidOperationException>();
                };
                return test.When(qty.IsValidQuantity());
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_remove_the_lineItem(
                LineItem lineItem,
                int qty)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(lineItem, (Quantity)qty);
                    order.Remove(lineItem);

                    order.Items.Should().NotContain(item => item.LineItem.Id == lineItem.Id);
                };
                return test.When(qty.IsValidQuantity());
            }

            public Property Should_raise_an_ItemRemovedFromOrderEvent(
               LineItem lineItem,
               int qty)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(lineItem, (Quantity)qty);
                    order.Remove(lineItem);

                    order.Events.Last().Should().BeOfType<ItemRemovedFromOrderEvent>();
                };
                return test.When(qty.IsValidQuantity());
            }
        }

        public class Place
        {
            [Fact]
            public void Should_fail_when_there_are_no_items_to_order()
            {
                new DraftOrder().Invoking(order => order.Place())
                    .Should().Throw<InvalidOperationException>();
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public void Should_fail_when_the_order_has_already_been_placed(
                LineItem lineItem,
                int qty)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    order.AddOrUpdate(lineItem, (Quantity)qty);
                    order.Place();
                    order.Invoking(o => o.Place())
                    .Should().Throw<InvalidOperationException>();
                };
                test.When(qty.IsValidQuantity());
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_raise_an_OrderPlacedEvent_for_each_supplier(
                List<LineItem> items)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    AddItemsToOrder(order, items);
                    order.Place();

                    var index = order.Items
                        .Select(item => item.LineItem.SupplierId)
                        .Distinct()
                        .Count();

                    order.Events.ToArray()[^index..]
                    .Should().AllBeOfType<OrderPlacedEvent>();
                };
                return test.When(items.Count > 0);
            }

            [Property(Arbitrary = new Type[] { typeof(LineItemGenerator) })]
            public Property Should_change_the_Status_to_Placed(
                List<LineItem> items)
            {
                Action test = () =>
                {
                    var order = new DraftOrder();
                    AddItemsToOrder(order, items);
                    order.Place();
                    order.Status.Should().Be(DraftOrderStatus.Placed);
                };
                return test.When(items.Count > 0);
            }

            private void AddItemsToOrder(DraftOrder order, IEnumerable<LineItem> items)
            {
                var rand = new System.Random();
                items = items.DistinctBy(lineItem => lineItem.Id);

                foreach (var item in items)
                {
                    order.AddOrUpdate(item, (Quantity)rand.Next(1, int.MaxValue));
                }
            }
        }
    }
}