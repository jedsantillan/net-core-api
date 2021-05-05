using System;
using System.Collections.Generic;
using System.Linq;
using FarmHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace FarmHub.Infrastructure.Services
{
    public static class TestUtils
    {
        public static DbContextOptions<T> GetInMemoryDbOptions<T>(string databaseName)
            where T : DbContext
        {
            var options = new DbContextOptionsBuilder<T>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            
            return options;
        }

        public static Mock<DbSet<T>> MockListToDbSet<T>(IEnumerable<T> list) where T : class
        {
            var mock = new Mock<DbSet<T>>();
            var queryable = list.AsQueryable();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mock;
        }
    }

    public abstract class DbContextTestBase : IDisposable
    {
        protected CatalogDbContext _context;
        protected string _databaseName;
        public DbContextTestBase()
        {
            _databaseName = Guid.NewGuid().ToString();
            CreateContext();
        }

        protected void CreateContext()
        {
            _context?.Dispose();
            var options = TestUtils.GetInMemoryDbOptions<CatalogDbContext>(_databaseName);
            _context = new CatalogDbContext(options);
            _context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}