namespace StockControl.Orders.Events
{
    public class OrderPlacedEvent: OrderEvent
    {
        public SupplierOrder Order { get; private set; }

        public OrderPlacedEvent(SupplierOrder order)
        {
            Order = order;
        }
    }
}
