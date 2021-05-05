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
    public class CustomerService : GenericRepository<Customer, CatalogDbContext>, ICustomerService
    {
        private ILogger<Customer> _logger;
        private CatalogDbContext _dbContext;

        public CustomerService(ILogger<Customer> logger, CatalogDbContext dbContext) : base(dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public new async Task<List<Customer>> GetAllListAsync()
        {
            var customerList = await _dbContext.Customers
                .ToListAsync();

            return customerList;
        }

        public new async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(s => s.Id == id);

            return customer;

        }
    }
}
