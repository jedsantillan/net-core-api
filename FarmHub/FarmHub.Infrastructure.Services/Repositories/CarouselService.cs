using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace FarmHub.Domain.Services.Repositories
{
    public class CarouselService : GenericRepository<Carousel, DbContext>, ICarouselService
    {
        private readonly CatalogDbContext _dbContext;
        public CarouselService(CatalogDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Carousel>> GetCarouselByType(CarouselType type)
        {
            return await _dbContext.Carousels.Include(c => c.Items).Where(c => c.Type == type).ToListAsync();
        }
    }
}