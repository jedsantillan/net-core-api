using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class FarmerService: GenericRepository<Farmer, CatalogDbContext>, IFarmerService
    {
        private ILogger<Farmer> _logger;

        public FarmerService(ILogger<Farmer> logger, CatalogDbContext context) :base(context)
        {
            _logger = logger;
        }
    }
}
