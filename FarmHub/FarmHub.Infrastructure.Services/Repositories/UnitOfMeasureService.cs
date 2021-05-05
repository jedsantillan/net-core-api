using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{

    public class UnitOfMeasureService : GenericRepository<UnitOfMeasure, CatalogDbContext>, IUnitOfMeasureService
    {
        private ILogger<UnitOfMeasureService> _logger;

        public UnitOfMeasureService(ILogger<UnitOfMeasureService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }
    }
}
