using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class HarvestPeriodService : GenericRepository<HarvestPeriod, CatalogDbContext>, IHarvestPeriodService
    {
        private ILogger<HarvestPeriodService> _logger;
        private CatalogDbContext _dbContext;

        public HarvestPeriodService(ILogger<HarvestPeriodService> logger, CatalogDbContext context) : base(context)
        {
            _logger = logger;
            _dbContext = context;
        }

        public new async Task<List<HarvestPeriod>> GetAllListAsync()
        {
            var harvestPeriodList = await _dbContext.HarvestPeriods
                .Include(i => i.Inventories)
                .ToListAsync();

            return harvestPeriodList;
        }

        public new async Task<HarvestPeriod> GetByIdAsync(int id)
        {
            var harvestPeriod = await _dbContext.HarvestPeriods
                .Include(i => i.Inventories)
                .FirstOrDefaultAsync(h => h.Id == id);

            return harvestPeriod;
        }
    }
}
