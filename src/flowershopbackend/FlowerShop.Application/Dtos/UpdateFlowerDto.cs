using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application.Dtos
{
    public class UpdateFlowerDto
    {
        [Required(ErrorMessage = "Id is required")]
        public long Id { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitCurrency { get; set; }
        // Currently, we use image URL by hardcoded for simplicity
        // Afterward, we can extend it to support image upload feature using blob storage
        public string ImageUrl { get; set; }
    }
}
