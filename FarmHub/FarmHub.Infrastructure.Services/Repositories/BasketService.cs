using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class BasketService : GenericRepository<Basket, CatalogDbContext>, IBasketService
    {
        private ILogger<BasketService> _logger;
        private CatalogDbContext _dbContext;

        public BasketService(ILogger<BasketService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public new async Task<List<Basket>> GetAllListAsync()
        {
            var basketList = await _dbContext.Baskets
                .Include(bp => bp.BasketProducts)
                    .ThenInclude(p => p.Product)
                .ToListAsync();

            return basketList;
        }

        public new async Task<Basket> GetByIdAsync(int id)
        {
            var basket = await _dbContext.Baskets
                .Include(bp => bp.BasketProducts)
                    .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(b => b.Id == id);

            return basket;
        }

        public void UpdateProducts(Basket basket, IEnumerable<int> productIds)
        {
            var enumerable = productIds as int[] ?? productIds.ToArray();
            var idsToAdd = enumerable.Except(basket.BasketProducts.Select(p => p.ProductId));
            var toRemove = basket.BasketProducts.Where(p => !enumerable.Contains(p.ProductId));
            _dbContext.RemoveRange(toRemove);
            _dbContext.AddRange(idsToAdd.Select(productIds => new BasketProduct
            {
                ProductId = productIds,
                BasketId = basket.Id
            }));
        }


    }
}
