using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StockControl.Orders.Data;
using StockControl.Orders.Services;

namespace StockControl.Orders.Application
{
    public static class IEnumarableHelpers
    {
        public static IEnumerable<T> Choose<T>(this IEnumerable<T?> source) where T:class
        {
            return source.Where(_ => _ != null)!;
        }

        public static IEnumerable<T> Choose<T>(this IEnumerable<T?> source) where T : struct
        {
            return source.Where(_ => _.HasValue).Select(_ => _.Value);
        }
    }
}
