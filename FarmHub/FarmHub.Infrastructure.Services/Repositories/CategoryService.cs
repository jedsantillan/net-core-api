using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class CategoryService : GenericRepository<Category, CatalogDbContext>, ICategoryService
    {
        private ILogger<CategoryService> _logger;

        public CategoryService(ILogger<CategoryService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

    }
}
