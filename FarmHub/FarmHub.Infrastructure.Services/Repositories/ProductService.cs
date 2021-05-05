using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace FarmHub.Domain.Services.Repositories
{
    public class ProductService : GenericRepository<Product, CatalogDbContext>, IProductService
    {
        private ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
        }

        public new async Task<List<Product>> GetAllListAsync()
        {
            var productList = await _dbContext.Products
                .Include(c => c.Category)
                .Include(i => i.Images)
                .Include(u => u.UnitOfMeasure)
                .Include(d => d.Discount)
                .Include(p => p.ProductPortions)
                .ThenInclude(p => p.Portion)
                .Include(p => p.ProductTags)
                .ToListAsync();
            return productList;
        }

        public new async Task<Product> GetByIdAsync(int id)
        {
            var product = await _dbContext.Products
                .Include(c => c.Category)
                .Include(i => i.Images)
                .Include(u => u.UnitOfMeasure)
                .Include(d => d.Discount)
                .Include(p => p.ProductPortions)
                .ThenInclude(p => p.Portion)
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        }

        public async Task<Product> GetByIdProductPortionAsync(int productId, int portionId)
        {
            var product = await _dbContext.Products
                .Include(c => c.Category)
                .Include(u => u.UnitOfMeasure)
                .Include(d => d.Discount)
                .Where(p => p.Id == productId)
                .IncludeFilter(x => Enumerable.Where<ProductPortion>(x.ProductPortions, y => y.PortionId == portionId)
                    .Select(p => p.Portion))
                .FirstOrDefaultAsync();

            return product;
        }

        public async Task<List<Product>> GetAllPromotionListAsync()
        {
            var productDiscountList = await _dbContext.Products
                .Include(c => c.Category)
                .Include(i => i.MainImage)
                .Include(i => i.Images)
                .Include(u => u.UnitOfMeasure)
                .Include(d => d.Discount)
                .Include(p => p.ProductPortions)
                .ThenInclude(p => p.Portion)
                .Where(x => x.Discount.StartDate <= DateTime.Now
                            && x.Discount.EndDate >= DateTime.Now)
                .ToListAsync();

            return productDiscountList;
        }

        public async Task<List<Image>> GetAllImagesByProductIdAsync(int productId)
        {
            var productImages = await _dbContext.Images
                .Where(p => p.ProductId == productId)
                .ToListAsync();

            return productImages;
        }

        public void UpdatePortions(Product product, IEnumerable<int> portionIds)
        {
            if (portionIds == null) return;

            var enumerable = portionIds as int[] ?? portionIds.ToArray();
            var idsToAdd = enumerable.Except(product.ProductPortions.Select(pp => pp.PortionId));
            var toRemove = product.ProductPortions.Where(pp => !enumerable.Contains(pp.PortionId));
            _dbContext.RemoveRange(toRemove);
            _dbContext.AddRange(idsToAdd.Select(portionId => new ProductPortion
            {
                PortionId = portionId,
                ProductId = product.Id
            }));
        }

        public async void UpdatePricePortion(Product product, int portionId, double price)
        {
            var productPortion = product.ProductPortions.Where(x => x.PortionId == portionId).SingleOrDefault();

            productPortion.Price = price;
            _dbContext.Update(productPortion);
            var result = await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllByTagAsync(string featured, int? limit)
        {
            var products = _dbContext.Products
                .Include(p => p.ProductTags)
                .Where(p =>
                    p.ProductTags.Any(t => t.Name.ToUpper() == featured.ToUpper()));

            return limit.HasValue ? await products.Take(limit.Value).ToListAsync() : await products.ToListAsync();
        }

        public async Task<List<Product>> GetAllBySales(int limit)
        {
            return await _dbContext.Products
                .Include(p => p.ProductPortions)
                .ThenInclude(pp => pp.OrderItems)
                .Include(p => p.ProductPortions)
                .ThenInclude(pp => pp.Portion)
                .Include(c => c.Category)
                .Include(i => i.Images)
                .Include(u => u.UnitOfMeasure)
                .Include(d => d.Discount)
                .Include(p => p.ProductPortions)
                .OrderByDescending(p => p.ProductPortions.SelectMany(pp => pp.OrderItems).Count())
                .Take(limit)
                .ToListAsync();
        }
    }
}