using System;

namespace StockControl.Orders
{
    public class LineItem
    {
        public int Id { get; private set; }

        public int SupplierId { get; private set; }

        public Product Product { get; private set; }

        public decimal PriceInPounds { get; private set; }

        public double ItemsPerPack { get; private set; } = 1;

        public Quantity CalculateTotalPacks(int totalItems) =>
            (Quantity)(totalItems * ItemsPerPack);

        public Quantity CalculateQuantityToOrder(int totalItems) =>
            (Quantity)(
            CalculateTotalPacks(totalItems) *
            Product.OrderingStrategy.Adjustment);

        public Quantity CalculateTotalItems(int totalPacks) =>
            (Quantity)Math.Floor(totalPacks / ItemsPerPack);
    }
}
