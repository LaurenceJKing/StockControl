using Xunit;
using StockControl.Orders;
using AutoMapper;

namespace TestsFor.StockControl.Orders.Application
{
    public class AutoMapper_Tests
    {
        [Fact]
        public void Mapping_should_work()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps("StockControl.Orders.Application"));
            config.AssertConfigurationIsValid();
        }
    }
 }
