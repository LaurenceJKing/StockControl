using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Moq;
using StockControl.Orders;
using StockControl.Orders.Application;
using StockControl.Orders.Data;
using StockControl.Orders.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestHelpers;
using Xunit;

namespace TestsFor.StockControl.Orders.Application.Features
{
    public class CreateDraftOrder_Tests
    {
        [Theory, AutoMoqData]
        public async Task Order_should_be_created(
            List<Product> products,
            [Frozen] Mock<IFactory<IEnumerable<StockRequirement>, DraftOrder>> factory)
        {
            var db = FakeDbContext.InMemory<OrderContext>();
            db.Set<Product>().AddRange(products);
            db.SaveChanges();
            var order = new DraftOrder();

            var request = new CreateDraftOrder.Request()
            {
                RequiredStock = products
                .Select(p => new CreateDraftOrder.StockLevelSummary()
                {
                    ProductId = p.Id,
                    TargetStockLevel = 10,
                    CurrentStockLevel = 0
                })
            };

            factory
                .Setup(f => f.Create(
                    It.IsAny<IEnumerable<StockRequirement>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var config = new MapperConfiguration(cfg => cfg.AddMaps("StockControl.Orders.Application"));
            var mapper = new Mapper(config);
            var sut = new CreateDraftOrder.Handler(db, factory.Object, mapper);

            var response = await sut.Handle(request, default);

            factory.Verify(f => f.Create(
                It.IsAny<IEnumerable<StockRequirement>>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            order.Id.Should().NotBe(0);
        }
    }

    public class PlaceOrder_Tests
    {
        [Fact]
        public async Task Order_should_be_placed()
        {
            var order = new DraftOrder();
            var lineItem = new LineItem(new Product() { Name = "Amoxicilin" }, 0m);
            order.AddOrUpdate(lineItem, (Quantity)1);

            var db = FakeDbContext.InMemory<OrderContext>();
            db.DraftOrders.Add(order);
            db.Set<LineItem>().Add(lineItem);
            db.SaveChanges();

            var sut = new PlaceDraftOrder.Handler(db);

            await sut.Handle(new PlaceDraftOrder.Request
            {
                OrderId = order.Id
            },
            default);

            db.SupplierOrders.Count().Should().BeGreaterThan(0);
        }
    }
}
