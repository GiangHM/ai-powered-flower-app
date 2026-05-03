namespace FlowerShop.Application.Interfaces;

/// <summary>Provides image storage operations against Azure Blob Storage / Azurite emulator.</summary>
public interface IImageStorageService
{
    /// <summary>Uploads an image stream and returns its public URL.</summary>
    Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
}
