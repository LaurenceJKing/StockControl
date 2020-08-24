using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Data.Common;

namespace TestHelpers
{
    public static class FakeDbContext
    {
        public static TContext InMemory<TContext>()
            where TContext : DbContext
        {
            var options = new DbContextOptionsBuilder<TContext>()
        .UseSqlite(CreateInMemoryDatabase())
        .Options;

            var db = (TContext)Activator.CreateInstance(
                typeof(TContext),
                new object[] { options });

            db.Database.EnsureCreated();
            return db;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }
    }
}
