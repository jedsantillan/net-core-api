using System.Collections.Generic;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;

namespace FarmHub.Application.Services.Repositories
{
    public interface IBasketService : IGenericRepository<Basket>
    {
        void UpdateProducts(Basket basket, IEnumerable<int> productIds);
    }
}
