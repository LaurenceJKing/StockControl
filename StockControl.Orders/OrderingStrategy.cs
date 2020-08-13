namespace StockControl.Orders
{
    public class OrderingStrategy
    {
        public int MinimumOrderQuantity { get; private set; } = 1;
        public double Adjustment { get; private set; } = 1;
    }
}