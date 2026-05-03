namespace FlowerShop.Application.Dtos;

/// <summary>Result of GPT-4o vision analysis for an uploaded flower image.</summary>
/// <param name="FlowerType">Botanical type or genus (e.g. Rosa, Tulipa).</param>
/// <param name="CommonName">Common name of the flower (e.g. Red Rose, Tulip).</param>
/// <param name="NotableCharacteristics">Notable visual characteristics described by the model.</param>
public record FlowerImageDescriptionDto(
    string FlowerType,
    string CommonName,
    string NotableCharacteristics);
