namespace StockControl.Orders
{
    public class SupplierOrderItem
    {
        public SupplierOrderItem(
            LineItem lineItem,
            Quantity quantityOrdered)
        {
            LineItem = lineItem;
            QuantityOrdered = quantityOrdered;
            QuantityAwaitingDelivery = quantityOrdered;
        }

        public LineItem LineItem { get; private set; }
        public Quantity QuantityOrdered { get; private set; }
        public Quantity QuantityDelivered { get; private set; }
        public Quantity QuantityAwaitingDelivery { get; private set; }

        internal void AddDelivery(Quantity quantityDelivered)
        {
            if (quantityDelivered < 1) return;

            QuantityDelivered += quantityDelivered;

            UpdateQuantityAwaitngDelivery();
        }

        private void UpdateQuantityAwaitngDelivery() {
            QuantityAwaitingDelivery = (Quantity)(
                    QuantityOrdered -
                    QuantityDelivered);
        }
    }
}
