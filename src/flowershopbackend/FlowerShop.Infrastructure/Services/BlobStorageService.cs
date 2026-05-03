using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FlowerShop.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FlowerShop.Infrastructure.Services;

/// <summary>Azure Blob Storage implementation of <see cref="IImageStorageService"/>.</summary>
public class BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger) : IImageStorageService
{
    private const string ContainerName = "flower-images";

    /// <inheritdoc/>
    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        logger.LogInformation("Uploaded image blob {BlobName} to container {Container}", blobName, ContainerName);
        return blobClient.Uri.ToString();
    }
}
