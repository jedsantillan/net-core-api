using System.Collections.Generic;
using System.Threading.Tasks;
using Adyen.Model.Nexo;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;

namespace FarmHub.Application.Services.Repositories
{
    public interface IProductService : IGenericRepository<Product>
    {
        Task<Product> GetByIdProductPortionAsync(int productId, int portionId);
        Task<List<Product>> GetAllPromotionListAsync();
        Task<List<Image>> GetAllImagesByProductIdAsync(int productId);

        void UpdatePortions(Product product, IEnumerable<int> portionIds);
        void UpdatePricePortion(Product product, int portionId, double price);

        Task<List<Product>> GetAllByTagAsync(string featured, int? limit);
        Task<List<Product>> GetAllBySales(int limit);
    }
}