using StockControl.Orders.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockControl.Orders
{
    public class DraftOrder
    {
        private readonly List<DraftOrderItem> _items = new List<DraftOrderItem>();

        private readonly List<OrderEvent> _events = new List<OrderEvent>();

        public int Id { get; internal set; }

        public DraftOrderStatus Status { get; private set; }

        public IEnumerable<DraftOrderItem> Items => _items;

        public IEnumerable<OrderEvent> Events => _events;

        public void AddOrUpdate(
            LineItem lineItem,
            Quantity quantity,
            string? notes = null)
        {
            if (Status != DraftOrderStatus.Draft)
                throw OrderAlreadyPlacedException();

            if (Contains(lineItem)) Remove(lineItem);

            _items.Add(new DraftOrderItem(lineItem, quantity));
            _events.Add(new ItemAddedToOrderEvent(lineItem, quantity, notes));
        }

        public void Remove(LineItem lineItem, string? notes = null)
        {
            if (Status != DraftOrderStatus.Draft)
                throw OrderAlreadyPlacedException();

            if (!Contains(lineItem))
                throw OrderDoesNotContainLineItemException(lineItem);

            _items.RemoveAll(item => item.LineItem.Id == lineItem.Id);
            _events.Add(new ItemRemovedFromOrderEvent(lineItem, notes));
        }

        public void Place()
        {
            if (Status != DraftOrderStatus.Draft)
                throw OrderAlreadyPlacedException();

            if(!Items.Any())
            throw OrderContainsNoItemsException();

            _events.AddRange(Items
                .GroupBy(item => item.LineItem.SupplierId)
                .Select(group => new OrderPlacedEvent(
                    new SupplierOrder(group.Key, Id, group))));

            Status = DraftOrderStatus.Placed;
        }

        public bool Contains(LineItem lineItem, out DraftOrderItem item)
        {
            item = Items.SingleOrDefault(_ => _.LineItem.Id == lineItem.Id);
            return item != null;
        }

        public bool Contains(LineItem lineItem) => Contains(lineItem, out var _);

        private Exception OrderAlreadyPlacedException() =>
            new InvalidOperationException(
                $"{nameof(DraftOrder)} {Id} has already been placed and cannot be updated." +
                "\r\n\r\n" +
                $"Consider creating a new {nameof(DraftOrder)}.");

        private Exception OrderDoesNotContainLineItemException(LineItem lineItem) => 
            new InvalidOperationException(
                $"{nameof(DraftOrder)} {Id} does not contain the {nameof(LineItem)} {lineItem.Id}" +
                "\r\n\r\n" +
                $"Consider adding the {nameof(LineItem)} using the {nameof(AddOrUpdate)} method.");

        private Exception OrderContainsNoItemsException() =>
            new InvalidOperationException(
                $"{nameof(DraftOrder)} {Id} does not contain any items" +
                "\r\n\r\n" +
                $"Consider adding items using the {nameof(AddOrUpdate)} method.");
    }

    public enum DraftOrderStatus
    {
        Draft,
        Placed
    }
}
