using FlowerShop.Application.Dtos;
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
        [ProducesResponseType(typeof(PagedResult<FlowerResponseItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActiveFlowers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            _logger.LogInformation("Getting active flowers page={Page} pageSize={PageSize}", page, pageSize);
            var result = await _flowerService.GetAllActiveFlowersPagedAsync(page, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("Flowers/{id}")]
        public async Task<IActionResult> GetFlowerById(long id)
        {
            _logger.LogInformation("Getting flower with id: {Id}", id);
            var flower = await _flowerService.GetFlowerByIdAsync(id);
            if (flower == null)
            {
                return NotFound();
            }
            return Ok(flower);
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

        /// <summary>
        /// Validates each cart item against current flower stock and active status.
        /// Returns per-item status: available, out_of_stock, inactive, or not_found.
        /// </summary>
        [HttpPost("Flowers/validate-cart")]
        public async Task<IActionResult> ValidateCart([FromBody] CartValidationRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating cart with {ItemCount} item(s)", request.Items?.Count ?? 0);

            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest("Cart must contain at least one item.");
            }

            if (request.Items.Any(i => i.Quantity <= 0))
            {
                return BadRequest("Each cart item must have a quantity greater than zero.");
            }

            var result = await _flowerService.ValidateCartAsync(request, cancellationToken);
            return Ok(result);
        }
    }
}
