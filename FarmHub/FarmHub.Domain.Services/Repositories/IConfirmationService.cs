using System;
using System.Threading.Tasks;
using FarmHub.Data.Models;
using FarmHub.Data.Repository;

namespace FarmHub.Application.Services.Repositories
{
    public interface IConfirmationService : IGenericRepository<ConfirmationEmail>
    {
        Task<ConfirmationResult> ValidateGuid(Guid guid, ConfirmationEmailType type);
    }

    public enum ConfirmationResult
    {
        Success,
        Expired,
        NonExistent
    }
}