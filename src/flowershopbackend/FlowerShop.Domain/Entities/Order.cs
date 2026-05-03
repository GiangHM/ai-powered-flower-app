using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Domain.Entities
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>ID of the user who placed this order; null for guest checkout.</summary>
        public long? UserId { get; set; }

        public string DeliveryName { get; set; } = string.Empty;
        public string DeliveryEmail { get; set; } = string.Empty;
        public string DeliveryPhone { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Creates a new order, validates inputs, and computes the total amount.
        /// </summary>
        public static Order PlaceOrder(
            long? userId,
            string deliveryName,
            string deliveryEmail,
            string deliveryPhone,
            IEnumerable<(long FlowerId, string FlowerName, int Quantity, decimal UnitPrice)> items)
        {
            if (string.IsNullOrWhiteSpace(deliveryName))
                throw new ArgumentException("Delivery name is required.", nameof(deliveryName));
            if (string.IsNullOrWhiteSpace(deliveryEmail))
                throw new ArgumentException("Delivery email is required.", nameof(deliveryEmail));
            if (string.IsNullOrWhiteSpace(deliveryPhone))
                throw new ArgumentException("Delivery phone is required.", nameof(deliveryPhone));

            var orderItemList = items.ToList();
            if (orderItemList.Count == 0)
                throw new ArgumentException("An order must contain at least one item.", nameof(items));

            var orderItems = orderItemList.Select(i => new OrderItem
            {
                FlowerId = i.FlowerId,
                FlowerName = i.FlowerName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var total = orderItems.Sum(i => i.UnitPrice * i.Quantity);

            return new Order
            {
                UserId = userId,
                DeliveryName = deliveryName.Trim(),
                DeliveryEmail = deliveryEmail.Trim(),
                DeliveryPhone = deliveryPhone.Trim(),
                Status = OrderStatus.Pending,
                TotalAmount = total,
                OrderDate = DateTime.UtcNow,
                Items = orderItems
            };
        }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled
    }
}
