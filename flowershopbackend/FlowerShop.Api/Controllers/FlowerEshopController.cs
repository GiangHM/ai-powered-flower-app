using FlowerShop.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowerEshopController : ControllerBase
    {
        private readonly ILogger<FlowerManagementController> _logger;
        private readonly IFlowerService _flowerService;

        public FlowerEshopController(ILogger<FlowerManagementController> logger
            , IFlowerService flowerService)
        {
            _logger = logger;
            _flowerService = flowerService;
        }

        [HttpGet("Flowers")]
        public async Task<IActionResult> GetAllActiveFlowers()
        {
            _logger.LogInformation("Getting all flowers");
            var flowers = await _flowerService.GetAllActiveFlowersAsync();
            return Ok(flowers);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string keyword)
        {
            _logger.LogInformation("Searching flowers with keyword: {Keyword}", keyword);

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword cannot be empty.");
            }

            var results = await _flowerService.SearchFlowersAsync(keyword);
            return Ok(results);
        }
    }
}
