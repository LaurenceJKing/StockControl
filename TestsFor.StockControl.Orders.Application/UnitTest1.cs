using Xunit;
using StockControl.Orders;
using AutoMapper;

namespace TestsFor.StockControl.Orders.Application
{
    public class UnitTest1
    {
        [Fact]
        public void Mapping_should_work()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps("StockControl.Orders.Application"));
            config.AssertConfigurationIsValid();
        }
    }

    public class QuantityConverter : IValueConverter<Quantity, int>
    {
        public int Convert(Quantity sourceMember, ResolutionContext context) =>
            (int)sourceMember;
    }
}
