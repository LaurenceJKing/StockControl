using AutoMapper;
using FluentAssertions;
using FsCheck.Xunit;
using Moq;
using StockControl.Orders;
using StockControl.Orders.Application;
using StockControl.Orders.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelpers;
using Xunit;

namespace TestsFor.StockControl.Orders.Application.Features
{
    public class AddOrUpdateDraftOrderItems_Tests
    {
        [Fact]
        public async Task Item_should_be_added_to_order()
        {
            var order = new DraftOrder();
            var lineItem = new LineItem(new Product() { Name = "Amoxicilin" }, 0m);
            var db = FakeDbContext.InMemory<OrderContext>();
            db.DraftOrders.Add(order);
            db.Set<LineItem>().Add(lineItem);
            db.SaveChanges();

            var config = new MapperConfiguration(cfg => cfg.AddMaps("StockControl.Orders.Application"));
            var mapper = new Mapper(config);

            var sut = new AddOrUpdateDraftOrderItem.Handler(db, mapper);

            var response = await sut.Handle(new AddOrUpdateDraftOrderItem.Request
            {
                OrderId = order.Id,
                LineItemId = lineItem.Id,
                Quantity = 1
            },
            default);

            order.Items.Should().Contain(i => i.LineItem.Id == lineItem.Id);
        }
    }
}
