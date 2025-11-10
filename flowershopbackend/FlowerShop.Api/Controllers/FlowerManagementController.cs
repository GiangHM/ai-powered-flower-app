using FlowerShop.Application.Dtos;
using FlowerShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlowerShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlowerManagementController : ControllerBase
    {
        private readonly ILogger<FlowerManagementController> _logger;
        private readonly IFlowerService _flowerService;
        private readonly IKafakaProducerService<string, string> _kafkaProducerService;

        public FlowerManagementController(ILogger<FlowerManagementController> logger
            , IFlowerService flowerService
            , [FromKeyedServices("vectorproducer")] IKafakaProducerService<string, string> kafkaProducerService)
        {
            _logger = logger;
            _flowerService = flowerService;
            _kafkaProducerService = kafkaProducerService;
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
    }
}
