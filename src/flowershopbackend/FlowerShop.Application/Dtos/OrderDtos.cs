using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application.Dtos
{
    public class OrderItemDto
    {
        [Required]
        public long FlowerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        /// <summary>ID of the authenticated user placing the order; null for guest checkout.</summary>
        public long? UserId { get; set; }

        [MaxLength(200)]
        public string? DeliveryName { get; set; }

        [EmailAddress]
        [MaxLength(320)]
        public string? DeliveryEmail { get; set; }

        [MaxLength(50)]
        public string? DeliveryPhone { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public long Id { get; set; }
        public long FlowerId { get; set; }
        public string FlowerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderResponseDto
    {
        public long Id { get; set; }

        /// <summary>ID of the user who placed the order; null for guest orders.</summary>
        public long? UserId { get; set; }

        public string DeliveryName { get; set; } = string.Empty;
        public string DeliveryEmail { get; set; } = string.Empty;
        public string DeliveryPhone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemResponseDto> Items { get; set; } = new();
    }

    /// <summary>Payload for updating an order's status (admin use).</summary>
    public class UpdateOrderStatusDto
    {
        /// <summary>
        /// New status for the order.
        /// Valid values: Pending, Confirmed, Shipped, Delivered, Cancelled.
        /// </summary>
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
