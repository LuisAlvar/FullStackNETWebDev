using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Imager.Server.Services;

public class AzureBlobStorageService
{
  private readonly BlobServiceClient _blobServiceClient;
  private readonly string _containerName;

  public AzureBlobStorageService(BlobServiceClient blobServiceClient, IConfiguration config)
  {
    _blobServiceClient = blobServiceClient;
    _containerName = config["AzureBlob:ContainerName"]!;
  }

  public async Task<string> UploadAsync(IFormFile file, string? fileName = null)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

    var blobName = fileName ?? $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    var blobClient = containerClient.GetBlobClient(blobName);

    await using var stream = file.OpenReadStream();
    await blobClient.UploadAsync(stream, new BlobHttpHeaders {
      ContentType = file.ContentType
    });

    return blobName; // this is your "id" for later retrieval
  }

  public async Task<Stream?> GetBlobStreamAsync(string blobName)
  {
    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
    var blobClient = containerClient.GetBlobClient(blobName);
    if (!await blobClient.ExistsAsync()) return null;
    var download = await blobClient.DownloadStreamingAsync();
    return download.Value.Content;
  }



}
