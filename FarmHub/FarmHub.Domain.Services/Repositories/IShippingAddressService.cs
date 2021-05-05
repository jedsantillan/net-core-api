using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmHub.Application.Services.Repositories
{
    public interface IShippingAddressService: IGenericRepository<ShippingAddress>
    {
        Task<List<ShippingAddress>> GetAllAddressByCustomerIdAsync(int customerId);
    }
}
