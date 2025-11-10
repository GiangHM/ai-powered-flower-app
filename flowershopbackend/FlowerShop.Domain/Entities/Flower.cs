using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowerShop.Domain.Entities
{
    public class Flower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public FlowerPricing UnitPrice { get; set; }
        public FlowerStock Stock { get; set; }
        public string FlowerName { get; set; }
        public string FlowerImageUrl { get; set; }
        public string FlowerDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual FlowerCategory Category { get; set; }
    }
}
