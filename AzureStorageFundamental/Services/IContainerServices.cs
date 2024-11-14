namespace AzureStorageFundamental.Services
{
    public interface IContainerServices
    {
        Task<List<string>> GetAllContainerAndBlobs();
        Task<List<string>> GetAllContainer();

        Task CreateContainer(string containerName);
        Task DeleteContainer(string containerName);
    }
}
