using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestsFor.StockControl.Orders.Application
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
        : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
