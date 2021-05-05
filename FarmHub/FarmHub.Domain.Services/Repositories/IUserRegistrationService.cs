using System.Collections.Generic;
using System.Threading.Tasks;
using FarmHub.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FarmHub.Application.Services.Repositories
{
    public interface IUserRegistrationService
    {
        Task<AddOrUpdateUserResult> CreateCustomerAndUserAsync(string modelEmail, string modelUserName, string modelPassword, Customer customerEntity);
        Task<AddOrUpdateUserResult> UpdateCustomerAndUserAsync(Customer updatedEntity, AuthUser identityUser, string modelCurrentPassword, string modelPassword);
        Task<AddOrUpdateUserResult> CreateUserAsync(string email, string userName, string password);

        Task<AuthUser> GetUserByEmailAsync(string email);
        Task<AuthUser> GetUserByIdAsync(int id);
        Task<AuthUser> GetUserByUserNameAsync(string username);
    }
    
    public class AddOrUpdateUserResult
    {
        public AuthUser AuthUser { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }

        public AddOrUpdateUserResult(AuthUser authUser, IEnumerable<IdentityError> errors)
        {
            AuthUser = authUser;
            Errors = errors;
        }
    }
}