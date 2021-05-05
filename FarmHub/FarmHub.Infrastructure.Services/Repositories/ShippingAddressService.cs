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
    public class ShippingAddressService : GenericRepository<ShippingAddress,CatalogDbContext>,IShippingAddressService
    {
        private ILogger<ShippingAddress> _logger;
        private CatalogDbContext _dbContext;

        public ShippingAddressService(ILogger<ShippingAddress> logger, CatalogDbContext dbContext) :base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public new async Task<List<ShippingAddress>> GetAllListAsync()
        {
            var shippingAddressList = await _dbContext.ShippingAddresses
                .Include(c => c.Customer).ToListAsync();

            return shippingAddressList;
        }

        public new async Task<ShippingAddress> GetByIdAsync(int id)
        {
            var shippingAddress = await _dbContext.ShippingAddresses
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(s => s.Id == id);

            return shippingAddress;

        }

        public async Task<List<ShippingAddress>> GetAllAddressByCustomerIdAsync(int customerId)
        {
            var shippingAddressList = await _dbContext.ShippingAddresses
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();

            return shippingAddressList;
        }


    }
}
