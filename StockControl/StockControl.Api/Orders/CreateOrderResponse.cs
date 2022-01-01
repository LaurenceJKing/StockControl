namespace StockControl.Api.Orders
{
    public class CreateOrderResponse : IEquatable<CreateOrderResponse?>
    { 
        public string Id { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            return Equals(obj as CreateOrderResponse);
        }

        public bool Equals(CreateOrderResponse? other)
        {
            return other != null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
