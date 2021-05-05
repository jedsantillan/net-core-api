using System;
using System.Threading.Tasks;
using FarmHub.Application.Services.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FarmHub.Domain.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly ILogger<AzureStorageService> _logger;
        private readonly CloudBlobContainer _blobContainer;

        public AzureStorageService(IOptions<AzureStorageConfig> config, ILogger<AzureStorageService> logger)
        {
            _logger = logger;
            var storageConfig = config.Value;
            CloudStorageAccount storageAccount;
            var storageCredentials = new StorageCredentials(storageConfig.AccountName, storageConfig.AccountKey);

            if (storageConfig.BlobEndpoint == null)
            {
                storageAccount = new CloudStorageAccount(storageCredentials, true);
            }
            else
            {
                storageAccount = new CloudStorageAccount(
                    storageCredentials,
                    storageConfig.BlobEndpoint,
                    storageConfig.QueueEndpoint,
                    storageConfig.TableEndpoint,
                    null);
            }

            var blobClient = storageAccount.CreateCloudBlobClient();
            _blobContainer = blobClient.GetContainerReference(storageConfig.Container);
            _blobContainer.CreateIfNotExistsAsync().Wait();
        }

        public async Task<string> UploadFile(IFormFile file, string blobFolderName)
        {
            try
            {
                blobFolderName = blobFolderName + "/" + file.FileName;
                var cloudBlock = _blobContainer.GetBlockBlobReference(blobFolderName);
                cloudBlock.Properties.ContentType = GetContentType(file.FileName);

                await using (var fileStream = file.OpenReadStream())
                {
                    await cloudBlock.UploadFromStreamAsync(fileStream);
                }

                return cloudBlock.Uri.AbsoluteUri;
            }
            catch(Exception ex)
            {
                _logger.LogError("Azure Storage Error. Exception: " + ex.Message);
                return null;
            }
        }

        private string GetContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
