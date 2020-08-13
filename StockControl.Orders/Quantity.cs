using System;

namespace StockControl.Orders
{
    public struct Quantity : IComparable<Quantity>, IEquatable<Quantity>
    {
        private readonly int _qty;

        public Quantity(int qty)
        {
            _qty = Math.Max(qty, 0);
        }

        public int CompareTo(Quantity other) => _qty.CompareTo(other._qty);

        public bool Equals(Quantity other) => _qty == other._qty;

        public static Quantity None = (Quantity)0;


        public static implicit operator int(Quantity quantity) =>
            quantity._qty;

        public static explicit operator Quantity(int quantity) =>
            new Quantity(quantity);

        public static explicit operator Quantity(decimal quantity) =>
            new Quantity((int)Math.Ceiling(quantity));
    }
}
