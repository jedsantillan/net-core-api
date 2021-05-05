using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class OrderItemService : GenericRepository<OrderItem, CatalogDbContext>, IOrderItemService
    {
        private ILogger<OrderItem> _logger;
        private CatalogDbContext _dbContext;

        public OrderItemService(ILogger<OrderItem> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
