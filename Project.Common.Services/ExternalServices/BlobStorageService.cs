using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Project.Common.Services.Helpers;
using Project.Common.Services.Models;

namespace Project.Common.Services.ExternalServices;

public interface IBlobStorageService
{
    Task<byte[]> GetFileAsync(string fileName, ContainerName containerName, CancellationToken cancellationToken);
    Task UploadFileAsync(string fileName, ContainerName containerName, Stream fileStream, CancellationToken cancellationToken);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly string _connectionString;

    public BlobStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<byte[]> GetFileAsync(string fileName, ContainerName containerName, CancellationToken cancellationToken)
    {
        var storageClient = new BlobServiceClient(_connectionString);
        var containerClient = storageClient.GetBlobContainerClient(containerName.GetDescription());
        var blobClient = containerClient.GetBlobClient(fileName);

        BlobDownloadInfo blobDownloadInfo;

        using (var ms = new MemoryStream())
        {
            blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken).ConfigureAwait(false);
            await blobDownloadInfo.Content.CopyToAsync(ms);
            ms.Position = 0;

            return ms.ToArray();
        }
    }

    public async Task UploadFileAsync(string fileName, ContainerName containerName, Stream fileStream, CancellationToken cancellationToken)
    {
        var storageClient = new BlobServiceClient(_connectionString);
        var containerClient = storageClient.GetBlobContainerClient(containerName.GetDescription());
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(fileStream, true, cancellationToken).ConfigureAwait(false);
    }
}