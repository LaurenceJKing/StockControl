namespace StockControl.Orders.Services
{
    public class StockRequirement
    {
        public StockRequirement(
            Product product,
            Quantity targetStockLevel,
            Quantity currentStockLevel,
            Quantity quantityAwaitingDelivery)
        {
            Product = product;
            TargetStockLevel = targetStockLevel;
            CurrentStockLevel = currentStockLevel;
            QuantityAwaitingDelivery = quantityAwaitingDelivery;
        }

        public Product Product { get; }

        public Quantity TargetStockLevel { get; }

        public Quantity CurrentStockLevel { get; }

        public Quantity QuantityAwaitingDelivery { get; }

        public Quantity QuantityRequired =>
            (Quantity)(
            TargetStockLevel -
            CurrentStockLevel -
            QuantityAwaitingDelivery);
    }
}
