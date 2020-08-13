using FluentAssertions;
using FsCheck.Xunit;
using StockControl.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestsFor.StockControl.Orders
{
    public class Quantity_Tests
    {
        [Property]
        public void Quantity_can_never_be_less_than_0(int quantity)
        {
            ((Quantity)quantity).Should().BeGreaterOrEqualTo((Quantity)0);
        }

        [Property]
        public void Identical_quantities_should_be_equal(int quantity)
        {
            ((Quantity)0).Should().Equals((Quantity)quantity);
        }
    }
}
