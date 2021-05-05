using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FarmHub.Application.Services.Infrastructure
{
    public interface IAzureStorageService
    {
        Task<string> UploadFile(IFormFile file, string blobFolderName);
    }
}
