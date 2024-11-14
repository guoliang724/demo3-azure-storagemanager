
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureStorageFundamental.Models;

namespace AzureStorageFundamental.Services
{
    public class BlobService : IBlobServices
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient) {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();
        }

        public  async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs =  blobContainerClient.GetBlobsAsync();
            var blobsString = new List<string>();
            await foreach (var blob in blobs) {
                blobsString.Add(blob.Name); 
            }
            return blobsString;
        }

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            var blobList = new List<Blob>();
            string sasContainerSignature = "";

            if (blobContainerClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = blobContainerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                sasContainerSignature = blobContainerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }



            await foreach (var item in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(item.Name);
                Blob blobIndividual = new()
                {
                    Uri = blobClient.Uri.AbsoluteUri + "?" + sasContainerSignature
                };

                BlobProperties properties = await blobClient.GetPropertiesAsync();
                if (properties.Metadata.ContainsKey("title"))
                {
                    blobIndividual.Title = properties.Metadata["title"];
                }
                if (properties.Metadata.ContainsKey("comment"))
                {
                    blobIndividual.Comment = properties.Metadata["comment"];
                }
                blobList.Add(blobIndividual);
            }

            return blobList;
        }


        public async Task<string> GetBlob(string blobName, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string blobName, IFormFile formFile, string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = formFile.ContentType,
            };

            var result = await blobClient.UploadAsync(formFile.OpenReadStream(), httpHeaders);

            if (result != null) return true;
            return false;
        }
    }
}
