using System.Text.Json.Serialization;

namespace StockControl.Orders
{
    public record Order
    {
        public Order()
        {
            Id = Guid.NewGuid();
        }



        public Guid Id { get; }
    }
}