using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using StockControl.Api.Orders;
using FluentAssertions;
using System.Net;
using StockControl.Orders;

namespace StockControl.Api.Tests.EndpointTests.Orders
{
    public class Create
    {
        [Fact]
        public async Task Returns_201()
        {
            var response = await CreateOrder();
            var order = await Deserialize<CreateOrderResponse>(response.Content);

            order.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers?.Location?.ToString().Should().Be($"orders/{order!.Id}");
        }

        [Fact]
        public async Task Saves_the_new_order()
        {
            var response = await CreateOrder();
            var order = await Deserialize<CreateOrderResponse>(response.Content);

            _repository.Should().Contain(o => o.Id == order!.Id);
        }

        private async Task<HttpResponseMessage> CreateOrder()
        {
            var client = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddTransient<IOrderRepository>(_ => _repository);
                    });
                })
                .CreateClient();

            return await client.PostAsync("orders/create", null);
        }

        private async Task<T?> Deserialize<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        private readonly FakeOrderRepository _repository = new FakeOrderRepository();
    }
}
