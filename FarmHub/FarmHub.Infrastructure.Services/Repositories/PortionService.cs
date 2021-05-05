using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;

namespace FarmHub.Domain.Services.Repositories
{
    public class PortionService : GenericRepository<Portion, CatalogDbContext>, IPortionService
    {
        public PortionService(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}