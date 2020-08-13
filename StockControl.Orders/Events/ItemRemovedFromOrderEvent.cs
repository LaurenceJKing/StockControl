namespace StockControl.Orders.Events
{
    public class ItemRemovedFromOrderEvent: OrderEvent
    {
        public LineItem LineItem { get; private set; }

        public ItemRemovedFromOrderEvent(
            LineItem lineItem,
            string? notes = null)
            : base(notes)
        {
            LineItem = lineItem;
        }
    }
}
