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
    public class InventoryService : GenericRepository<Inventory, CatalogDbContext>, IInventoryService
    {
        private ILogger<InventoryService> _logger;
        private CatalogDbContext _dbContext;
        
        public InventoryService(ILogger<InventoryService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public new async Task<List<Inventory>> GetAllListAsync()
        {
            var inventoryList = await _dbContext.Inventories
                .Include(h => h.HarvestPeriod)
                .Include(a => a.FarmerAssociation)
                .Include(p => p.Product)
                    .ThenInclude(u => u.UnitOfMeasure)
                .Include(p => p.Product)  
                    .ThenInclude(c => c.Category)
                .ToListAsync();

            return inventoryList;
        }

        public new async Task<Inventory> GetByIdAsync(int id)
        {
            var inventory = await _dbContext.Inventories
                .Include(h => h.HarvestPeriod)
                .Include(a => a.FarmerAssociation)
                .Include(p => p.Product)
                    .ThenInclude(u => u.UnitOfMeasure)
                .Include(p => p.Product)
                    .ThenInclude(c => c.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            return inventory;
        }
    }
}