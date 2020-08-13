using System;

namespace StockControl.Orders
{
    public class DraftOrderItem
    {
        internal DraftOrderItem(LineItem lineItem, Quantity quantity)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Quantity cannot be less than 1.");
            }

            LineItem = lineItem;
            Quantity = quantity;
        }

        public LineItem LineItem { get; private set; }
        public Quantity Quantity { get; private set; }
    }
}                                                                            