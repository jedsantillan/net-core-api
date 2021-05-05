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
    public class DiscountService : GenericRepository<Discount, CatalogDbContext>, IDiscountService
    {
        private ILogger<DiscountService> _logger;
        private CatalogDbContext _dbContext;


        public DiscountService(ILogger<DiscountService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public new async Task<List<Discount>> GetAllListAsync()
        {
            var discountList = await _dbContext.Discounts
                .Include(p => p.Product)
                .ToListAsync();

            return discountList;
        }

        public new async Task<Discount> GetByIdAsync(int id)
        {
            var discount = await _dbContext.Discounts
                .Include(p => p.Product)
                .FirstOrDefaultAsync(d => d.Id == id);

            return discount;
        }
    }
}
