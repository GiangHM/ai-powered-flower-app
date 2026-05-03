using FlowerShop.Application.Dtos;
using FlowerShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlowerShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class FlowerManagementController : ControllerBase
    {
        private const long MaxImageSizeBytes = 10 * 1024 * 1024; // 10 MB
        private static readonly string[] AllowedImageMimeTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];
        private readonly ILogger<FlowerManagementController> _logger;
        private readonly IFlowerService _flowerService;
        private readonly IOrderService _orderService;
        private readonly IKafakaProducerService<string, string> _kafkaProducerService;
        private readonly IImageStorageService _imageStorageService;
        private readonly IFlowerImageService _flowerImageService;

        public FlowerManagementController(ILogger<FlowerManagementController> logger
            , IFlowerService flowerService
            , IOrderService orderService
            , [FromKeyedServices("vectorproducer")] IKafakaProducerService<string, string> kafkaProducerService
            , IImageStorageService imageStorageService
            , IFlowerImageService flowerImageService)
        {
            _logger = logger;
            _flowerService = flowerService;
            _orderService = orderService;
            _kafkaProducerService = kafkaProducerService;
            _imageStorageService = imageStorageService;
            _flowerImageService = flowerImageService;
        }

        [HttpGet("Flowers")]
        public async Task<IActionResult> GetAllFlowers()
        {
            _logger.LogInformation("Getting all flowers");
            var flowers = await _flowerService.GetAllFlowersAsync();
            return Ok(flowers);
        }

        [HttpPost("Flowers")]
        public async Task<IActionResult> Create([FromBody] CreateFlowerDto request)
        {
            if (!ModelState.IsValid)
            {
                var messages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Invalid input: {messages}");
            }
            var results = await _flowerService.CreateFlowersAsync(request);

            return Ok(results);
        }

        [HttpPut("Flowers")]
        public async Task<IActionResult> Update([FromBody] UpdateFlowerDto request)
        {
            if (!ModelState.IsValid)
            {
                var messages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Invalid input: {messages}");
            }
            var results = await _flowerService.UpdateFlowerAsync(request);
            return Ok(results);
        }

        [HttpPut("Flowers/{id}/status/{status}")]
        public async Task<IActionResult> UpdateStatus(long id, bool status)
        {
            if (id <= 0)
            {
                return BadRequest($"Invalid input id");
            }

            var request = (id, status);
            var results = await _flowerService.UpdateFlowerStatusAsync(request);

            if (results != null && results.Id > 0)
            {
                var vectorRequest = new InitVectorDataRequest
                {
                    Action = status ? "Upsert" : "Delete",
                    Payload = new List<FlowerVectorDataRequestDto>
                    {
                        new FlowerVectorDataRequestDto
                        {
                            Id = results.Id,
                            Name = results.FlowerName,
                            Description = results.FlowerDescription,
                            UnitPrice = results.UnitPrice.Price.Amount,
                            UnitCurrency = results.UnitPrice.Price.Currency
                        }
                    }
                };
                var kafkaValue = JsonSerializer.Serialize(vectorRequest);
                _ = _kafkaProducerService.ProduceAsync("flower-vectors", results.Id.ToString(), kafkaValue);
            }
            return Ok();
        }

        [HttpDelete("Flowers/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var results = await _flowerService.DeleteFlowerAsync(id);
            return Ok(results);
        }

        /// <summary>Gets all orders (admin view).</summary>
        [HttpGet("Orders")]
        public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin: getting all orders");
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            return Ok(orders);
        }

        /// <summary>Uploads a flower image to Blob Storage and returns its public URL.</summary>
        [HttpPost("Flowers/upload-image")]
        [RequestSizeLimit(MaxImageSizeBytes)]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            if (file.Length > MaxImageSizeBytes)
                return BadRequest("File exceeds the 10 MB size limit.");

            if (!AllowedImageMimeTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
                return BadRequest($"Unsupported file type '{file.ContentType}'. Allowed: jpeg, png, gif, webp.");

            await using var stream = file.OpenReadStream();
            var url = await _imageStorageService.UploadAsync(stream, file.FileName, file.ContentType, cancellationToken);
            return Ok(new { url });
        }

        /// <summary>
        /// Accepts a multipart image upload and uses GPT-4o vision to identify the flower,
        /// returning its type, common name and notable characteristics.
        /// </summary>
        [HttpPost("describe-image")]
        [RequestSizeLimit(MaxImageSizeBytes)]
        public async Task<IActionResult> DescribeImage(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided.");

            if (file.Length > MaxImageSizeBytes)
                return BadRequest("File exceeds the 10 MB size limit.");

            if (!AllowedImageMimeTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
                return BadRequest($"Unsupported file type '{file.ContentType}'. Allowed: jpeg, png, gif, webp.");

            await using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, cancellationToken);
            var imageBytes = ms.ToArray();

            var result = await _flowerImageService.DescribeImageAsync(imageBytes, file.ContentType, cancellationToken);
            return Ok(result);
        }
    }
}
