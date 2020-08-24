using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockControl.Orders.Events;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace StockControl.Orders.Data.TypeConfigurations
{
    public class DraftOrderItemTypeConfiguration : IEntityTypeConfiguration<DraftOrderItem>
    {
        public void Configure(EntityTypeBuilder<DraftOrderItem> builder)
        {
            builder.Property<int>("DraftOrderId");

            builder.HasKey(new string[] { "DraftOrderId", "LineItemId" });

            builder
                .Property(d => d.Quantity)
                .HasConversion(new QuantityConverter());
        }
    }

    public class ProductTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.OwnsOne(p => p.OrderingStrategy);
        }
    }

    public class OrderEventTypeConfiguration : IEntityTypeConfiguration<OrderEvent>
    {
        public void Configure(EntityTypeBuilder<OrderEvent> builder)
        {
            builder.HasDiscriminator<string>("EventType")
                .HasValue<ItemAddedToOrderEvent>(nameof(ItemAddedToOrderEvent))
                .HasValue<ItemRemovedFromOrderEvent>(nameof(ItemRemovedFromOrderEvent))
                .HasValue<OrderPlacedEvent>(nameof(OrderPlacedEvent));
        }
    }

    public class SupplierOrderItemTypeConfiguration : IEntityTypeConfiguration<SupplierOrderItem>
    {
        public void Configure(EntityTypeBuilder<SupplierOrderItem> builder)
        {
            builder.Property<int>("SupplierOrderId");

            builder.HasKey(new string[] { "SupplierOrderId", "LineItemId" });
            builder.Property(s => s.QuantityOrdered).HasConversion(new QuantityConverter());
            builder.Property(s => s.QuantityDelivered).HasConversion(new QuantityConverter());
            builder.Property(s => s.QuantityAwaitingDelivery).HasConversion(new QuantityConverter());
        }
    }

    public class QuantityConverter: ValueConverter<Quantity, int>
    {
        public QuantityConverter() : base(qty => (int)qty, qty => (Quantity)qty)
        {
        }
    }
}
