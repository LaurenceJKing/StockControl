using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using MoreLinq;
using StockControl.Orders;
using StockControl.Orders.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace TestsFor.StockControl.Orders
{
    public static class LineItemGenerator
    {
        public static Arbitrary<LineItem> GenerateLineItem() =>
            Arb.From(Arb.Generate<int>().Two().Select(_ =>
            {
                var lineItem = new LineItem(new Product(), 0m);
                lineItem.SetProperty(_ => _.Id, _.Item1);
                lineItem.SetProperty(_ => _.Product, CreateProduct(_.Item2));
                return lineItem;
            }
            ));

        private static Product CreateProduct(int id)
        {
            var product = new Product();
            product.SetProperty(p => p.Id, id);
            return product;
        }
    }

    public static class Extensions
    {
        public static void SetProperty<T, TProp>(
            this T obj,
            Expression<Func<T,TProp>> property,
            TProp value)
        {
            var body = property.Body as MemberExpression;
            typeof(T).GetProperty(body.Member.Name).SetValue(obj, value);
        }
    }
}
