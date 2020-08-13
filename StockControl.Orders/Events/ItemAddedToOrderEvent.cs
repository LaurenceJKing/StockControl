namespace StockControl.Orders.Events
{
    public class ItemAddedToOrderEvent : OrderEvent
    {
        public ItemAddedToOrderEvent(
            LineItem lineItem,
            int quantity,
            string? notes = null)
            : base(notes)
        {
            LineItem = lineItem;
            Quantity = quantity;
        }

        private ItemAddedToOrderEvent() { }

        public LineItem LineItem { get; private set; }

        public int Quantity { get; private set; }
    }
}
                                                                                