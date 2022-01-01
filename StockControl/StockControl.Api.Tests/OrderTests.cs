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

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location?.ToString()
                .Should()
                .Be("orders/dc069aeb-5eb9-4f1b-9dba-bcc7d7b0c47d");

            repository.Should().Contain(new Order());
        }
    }
}