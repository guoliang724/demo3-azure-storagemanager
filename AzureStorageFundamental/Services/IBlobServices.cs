using AzureStorageFundamental.Models;

namespace AzureStorageFundamental.Services
{
    public interface IBlobServices
    {
        Task<string> GetBlob(string blobName, string containerName);

        Task<List<string>> GetAllBlobs(string containerName);
        Task<List<Blob>> GetAllBlobsWithUri(string containerName);

        Task<bool> UploadBlob(string blobName,IFormFile formFile,string containerName);
        Task<bool> DeleteBlob(string blobName,string containerName);
    }
}
