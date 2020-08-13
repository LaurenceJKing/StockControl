using System;

namespace StockControl.Orders.Events
{
    public abstract class OrderEvent
    {
        public int Id { get; private set; }
        public DateTime TimeStamp { get; private set; } = DateTime.UtcNow;

        public string? Notes { get; private set; }

        protected OrderEvent(string? notes = null)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }
    }
}
                                                                                