namespace FlowerShop.Application.Dtos
{
    /// <summary>
    /// Represents a single cart item to validate.
    /// </summary>
    public record CartItemDto(long FlowerId, int Quantity);

    /// <summary>
    /// Request body for the cart validation endpoint.
    /// </summary>
    public record CartValidationRequestDto(List<CartItemDto> Items);

    /// <summary>
    /// Status values returned by the cart validation endpoint.
    /// </summary>
    public static class CartValidationStatus
    {
        /// <summary>Flower is active and has sufficient stock.</summary>
        public const string Available = "available";

        /// <summary>Flower is active but stock is insufficient for the requested quantity.</summary>
        public const string OutOfStock = "out_of_stock";

        /// <summary>Flower exists but is not currently active.</summary>
        public const string Inactive = "inactive";

        /// <summary>No flower was found for the given ID.</summary>
        public const string NotFound = "not_found";
    }

    /// <summary>
    /// Validation result for a single cart item.
    /// </summary>
    public class CartItemValidationResult
    {
        /// <summary>The flower ID that was validated.</summary>
        public long FlowerId { get; set; }

        /// <summary>The quantity that was requested.</summary>
        public int RequestedQuantity { get; set; }

        /// <summary>
        /// Validation status. See <see cref="CartValidationStatus"/> for possible values.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response body for the cart validation endpoint containing per-item results.
    /// </summary>
    public class CartValidationResponseDto
    {
        /// <summary>Per-item validation results.</summary>
        public List<CartItemValidationResult> Results { get; set; } = [];
    }
}
