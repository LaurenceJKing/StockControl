namespace StockControl.Orders.Events
{
    public class OrderPlacedEvent: OrderEvent
    {
        private OrderPlacedEvent() { }
        public SupplierOrder Order { get; private set; }

        public OrderPlacedEvent(SupplierOrder order)
        {
            Order = order;
        }
    }
}
