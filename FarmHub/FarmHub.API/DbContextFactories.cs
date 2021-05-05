using System.IO;
using FarmHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FarmHub.API
{
    public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

            var connectionString = configuration.GetConnectionString("CatalogConnectionString");

            dbContextBuilder.UseSqlServer(configuration.GetConnectionString("CatalogConnectionString"),
                b => b.MigrationsAssembly("FarmHub.Data"));

            return new CatalogDbContext(dbContextBuilder.Options);
        }
    }
}