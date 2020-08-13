using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using StockControl.Orders.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace TestsFor.StockControl.Orders
{
    public static class MockDbContext
    {
        public static OrderContext InMemory()
        {
            var options = new DbContextOptionsBuilder<OrderContext>()
        .UseSqlite(CreateInMemoryDatabase())
        .Options;

            return new OrderContext(options);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }
    }
}
