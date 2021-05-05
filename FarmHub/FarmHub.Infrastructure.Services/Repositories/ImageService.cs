using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class ImageService : GenericRepository<Image, CatalogDbContext>, IImageService
    {
        private ILogger<ImageService> _logger;
        private CatalogDbContext _dbContext;

        public ImageService(ILogger<ImageService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
