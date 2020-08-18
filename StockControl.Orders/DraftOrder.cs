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
            EnsureIsDraft();
            EnsureValidQuantity(quantity);

            if (Contains(lineItem)) Remove(lineItem);

            _items.Add(new DraftOrderItem(lineItem, quantity));
            _events.Add(new ItemAddedToOrderEvent(lineItem, quantity, notes));
        }

        public void Remove(LineItem lineItem, string? notes = null)
        {
            EnsureIsDraft();
            EnsureContains(lineItem);

            _items.RemoveAll(item => item.LineItem.Id == lineItem.Id);
            _events.Add(new ItemRemovedFromOrderEvent(lineItem, notes));
        }

        public void Place()
        {
            EnsureIsDraft();
            EnsureAnyItems();

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

        private void EnsureValidQuantity(Quantity quantity)
        {
            if (quantity >= 1) return;
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                quantity,
                $"Quantities of 0 or less cannot be added to a {nameof(DraftOrder)}.");
        }

        private void EnsureIsDraft()
        {
            if (Status == DraftOrderStatus.Draft) return;
            throw new InvalidOperationException(
                $"{nameof(DraftOrder)} {Id} " +
                $"does not have a {nameof(Status)} of {nameof(DraftOrderStatus.Draft)}." +
                "\r\n\r\n" +
                $"Consider creating a new {nameof(DraftOrder)}.");
        }

        private void EnsureContains(LineItem lineItem)
        {
            if (Contains(lineItem)) return;
            throw new InvalidOperationException(
                $"{nameof(DraftOrder)} {Id} does not contain the {nameof(LineItem)} {lineItem.Id}");
        }

        private void EnsureAnyItems()
        {
            if (Items.Any()) return;
            throw new InvalidOperationException(
                $"{nameof(DraftOrder)} {Id} does not contain any items.");
        }
    }

    public enum DraftOrderStatus
    {
        Draft,
        Placed
    }
}
