using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class FarmerAssociationService : GenericRepository<FarmerAssociation, CatalogDbContext>, IFarmerAssociationService
    {
        private ILogger<FarmerAssociationService> _logger;
        
        public FarmerAssociationService(ILogger<FarmerAssociationService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }
    }
}