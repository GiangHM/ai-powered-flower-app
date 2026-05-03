using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long FlowerId { get; set; }
        public string FlowerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
