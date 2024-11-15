﻿
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureStorageFundamental.Services
{
    public class ContainerService : IContainerServices
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ContainerService(BlobServiceClient blobServiceClient) { 
           _blobServiceClient = blobServiceClient;
        }
        public async Task CreateContainer(string containerName)
        {
          BlobContainerClient blobContainerClient =   _blobServiceClient.GetBlobContainerClient(containerName);
          await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerName = new List<string>();
            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync()) {
                containerName.Add(blobContainerItem.Name);
            }
            return containerName;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobNames = new();
            containerAndBlobNames.Add("Account Name : " + _blobServiceClient.AccountName);
            containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");
            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("--" + blobContainerItem.Name);
                BlobContainerClient _blobContainer =
                      _blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);
                await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
                {
                    //get metadata
                    var blobClient = _blobContainer.GetBlobClient(blobItem.Name);
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    string blobToAdd = blobItem.Name;
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        blobToAdd += "(" + blobProperties.Metadata["title"] + ")";
                    }

                    containerAndBlobNames.Add("------" + blobToAdd);
                }
                containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");

            }
            return containerAndBlobNames;
        }
    }
}
