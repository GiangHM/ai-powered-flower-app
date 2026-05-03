using FlowerShop.Application.Dtos;

namespace FlowerShop.Application.Interfaces;

/// <summary>Identifies a flower from an image using a vision-capable language model.</summary>
public interface IFlowerImageService
{
    /// <summary>
    /// Sends the image bytes to GPT-4o vision and returns flower identification details.
    /// </summary>
    /// <param name="imageBytes">Raw image bytes.</param>
    /// <param name="contentType">MIME type of the image (e.g. "image/jpeg").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identification result with flower type, common name and notable characteristics.</returns>
    Task<FlowerImageDescriptionDto> DescribeImageAsync(
        byte[] imageBytes,
        string contentType,
        CancellationToken cancellationToken = default);
}
