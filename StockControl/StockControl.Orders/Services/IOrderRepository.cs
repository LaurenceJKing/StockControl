namespace StockControl.Orders
{
    public interface IOrderRepository
    {
        Task Create(Order newOrder);
    }
}