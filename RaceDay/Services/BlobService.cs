using Azure.Storage.Blobs;

namespace RaceDay.Services;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName = "event-banners";

    public BlobService(IConfiguration config)
    {
        // This pulls "UseDevelopmentStorage=true" from your appsettings.json
        _connectionString = config.GetConnectionString("AzureBlobStorage")!;
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Create container if it doesn't exist and allow public access for reading images
        await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blobClient = containerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);

        return blobClient.Uri.ToString();
    }
}