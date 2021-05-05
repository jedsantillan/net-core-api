using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FarmHub.Domain.Services.Repositories
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly CatalogDbContext _dbContext;
        private readonly UserManager<AuthUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly ILogger<UserRegistrationService> _logger;

        public UserRegistrationService(CatalogDbContext dbContext, UserManager<AuthUser> userManager, ICustomerService customerService, ILogger<UserRegistrationService> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _customerService = customerService;
            _logger = logger;
        }
        
        public async Task<AddOrUpdateUserResult> CreateCustomerAndUserAsync(string modelEmail, string modelUserName, string modelPassword, Customer customerEntity)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var user = new AuthUser
            {
                Email = modelEmail,
                UserName = modelUserName,
            };
            customerEntity.AuthUser = user;
            // Todo Andrei: Test if customerentity has an auth user
            
            var result = await _userManager.CreateAsync(user, modelPassword);
            if (!result.Succeeded) return new AddOrUpdateUserResult(null, result.Errors);
            await _customerService.InsertAsync(customerEntity);
            await transaction.CommitAsync();
            return new AddOrUpdateUserResult(user, null);
        }

        public async Task<AddOrUpdateUserResult> UpdateCustomerAndUserAsync(Customer updatedEntity,
            AuthUser identityUser, string currentPassword, string newPassword)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            identityUser.Email = updatedEntity.ContactEmail;
            identityUser.UserName = updatedEntity.UserName;

            var result =  await _userManager.UpdateAsync(identityUser);

            if (!result.Succeeded) return new AddOrUpdateUserResult(null, result.Errors);

            if (!string.IsNullOrEmpty(newPassword))
            {
                var pwResult =
                    await _userManager.ChangePasswordAsync(identityUser, currentPassword, newPassword);

                if (!pwResult.Succeeded) return new AddOrUpdateUserResult(null, pwResult.Errors);
            }

            await _customerService.Update(updatedEntity);
            await transaction.CommitAsync();
            return null;
        }

        public async Task<AddOrUpdateUserResult> CreateUserAsync(string email, string userName, string password)
        {
            var user = new AuthUser
            {
                Email = email,
                UserName = userName,
            };
            
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded ? new AddOrUpdateUserResult(user, null) : new AddOrUpdateUserResult(null, result.Errors);
        }

        public async Task<AuthUser> GetUserByEmailAsync(string email)
        {
            return await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AuthUser> GetUserByIdAsync(int id)
        {
            return await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AuthUser> GetUserByUserNameAsync(string username)
        {
            return await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}