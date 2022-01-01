using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json;
using FluentAssertions;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using StockControl.Orders;
using StockControl.Api.Orders;

namespace StockControl.Api.Tests
{
    public class OrderTests
    {
        [Fact]
        public async Task CreateReturns201()
        {
            var repository = new FakeOrderRepository();

            var client = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddTransient<IOrderRepository>(_ => repository);
                });
            }).CreateClient();

            var response = await client.PostAsync("orders/create", null);

            var json = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<CreateOrderResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            order.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers?.Location?.ToString().Should().Be($"orders/{order!.Id}");

            repository.Should().Contain(o => o.Id.ToString() == order!.Id);
        }
    }
}