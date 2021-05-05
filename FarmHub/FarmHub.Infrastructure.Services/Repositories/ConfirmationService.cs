using System;
using System.Linq;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace FarmHub.Domain.Services.Repositories
{
    public class ConfirmationService : GenericRepository<ConfirmationEmail, CatalogDbContext>, IConfirmationService
    {
        public async Task<ConfirmationResult> ValidateGuid(Guid guid, ConfirmationEmailType confirmationEmailType)
        {
            var found = await _dbContext.ConfirmationEmails.FirstOrDefaultAsync(e => e.IsActive && e.Guid == guid && e.Type == confirmationEmailType);
            
            if (found == null) return ConfirmationResult.NonExistent;

            if (found.Expiration < DateTime.Now) return ConfirmationResult.Expired;

            var user = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Email == found.Email);

            if (user != null)
            {
                user.EmailConfirmed = true;
                _dbContext.Update(user);
            }
            
            found.IsActive = false;
            _dbContext.Update(found);
            await _dbContext.SaveChangesAsync();
            return ConfirmationResult.Success;
        }

        public ConfirmationService(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}