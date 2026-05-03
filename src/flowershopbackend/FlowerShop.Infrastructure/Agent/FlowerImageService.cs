using FlowerShop.Application.Dtos;
using FlowerShop.Application.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowerShop.Infrastructure.Agent;

/// <summary>
/// Uses a GPT-4o vision-capable <see cref="IChatClient"/> to identify a flower from an uploaded image.
/// </summary>
public class FlowerImageService(
    [FromKeyedServices("visionclient")] IChatClient chatClient,
    ILogger<FlowerImageService> logger) : IFlowerImageService
{
    private const string SystemPrompt =
        "You are a professional botanist and florist. " +
        "When given an image of a flower, identify it and respond ONLY with a valid JSON object " +
        "containing exactly these three fields: " +
        "\"flowerType\" (the botanical genus or family, e.g. Rosa), " +
        "\"commonName\" (the well-known common name, e.g. Red Rose), and " +
        "\"notableCharacteristics\" (a short description of the flower's visual features, colour, shape, and scent if known). " +
        "Do not include any markdown fences or extra text outside the JSON.";

    /// <inheritdoc/>
    public async Task<FlowerImageDescriptionDto> DescribeImageAsync(
        byte[] imageBytes,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        if (imageBytes.Length == 0)
            throw new ArgumentException("Image bytes must not be empty.", nameof(imageBytes));
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type must not be empty.", nameof(contentType));

        logger.LogInformation("Sending image ({Bytes} bytes) to vision model.", imageBytes.Length);

        var message = new ChatMessage(ChatRole.User,
        [
            new TextContent("Please identify this flower image:"),
            new DataContent(imageBytes, contentType)
        ]);

        var response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, SystemPrompt),
                message
            ],
            cancellationToken: cancellationToken);

        var text = response.Text ?? string.Empty;
        logger.LogInformation("Vision model responded with {Length} characters.", text.Length);

        return ParseResponse(text);
    }

    private static FlowerImageDescriptionDto ParseResponse(string text)
    {
        // Strip optional markdown code fences if the model returns them despite instructions.
        var trimmed = text.Trim();
        if (trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            var firstNewline = trimmed.IndexOf('\n');
            var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (firstNewline >= 0 && lastFence > firstNewline)
                trimmed = trimmed[(firstNewline + 1)..lastFence].Trim();
        }

        try
        {
            var raw = JsonSerializer.Deserialize<VisionRaw>(trimmed, JsonOptions);
            return new FlowerImageDescriptionDto(
                FlowerType: raw?.FlowerType ?? "Unknown",
                CommonName: raw?.CommonName ?? "Unknown",
                NotableCharacteristics: raw?.NotableCharacteristics ?? string.Empty);
        }
        catch (JsonException)
        {
            // Fallback: return the raw text as the notable characteristics.
            return new FlowerImageDescriptionDto("Unknown", "Unknown", trimmed);
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private sealed class VisionRaw
    {
        [JsonPropertyName("flowerType")]
        public string? FlowerType { get; set; }

        [JsonPropertyName("commonName")]
        public string? CommonName { get; set; }

        [JsonPropertyName("notableCharacteristics")]
        public string? NotableCharacteristics { get; set; }
    }
}
