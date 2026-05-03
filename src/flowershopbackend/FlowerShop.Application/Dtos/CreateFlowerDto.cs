using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application.Dtos
{
    public class CreateFlowerDto
    {
        [Required(ErrorMessage = "Flowername is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Flower description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Unit price is required")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "Unit currency is required")]
        public string UnitCurrency { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public long CategoryId { get; set; }
        // Currently, we use image URL by hardcoded for simplicity
        // Afterward, we can extend it to support image upload feature using blob storage
        public string ImageUrl { get; set; }
    }
}
