using FlowerShop.Application.Dtos;
using FlowerShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FlowerShop.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;
        private readonly IKafakaProducerService<string, string> _kafkaProducerService;

        public OrdersController(
            ILogger<OrdersController> logger,
            IOrderService orderService,
            [FromKeyedServices("vectorproducer")] IKafakaProducerService<string, string> kafkaProducerService)
        {
            _logger = logger;
            _orderService = orderService;
            _kafkaProducerService = kafkaProducerService;
        }

        /// <summary>Places a new order. Supports guest checkout (UserId is optional).</summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("nameid")
                    ?? User.FindFirstValue("sub");

                if (!long.TryParse(userIdClaim, out var authenticatedUserId))
                    return Unauthorized("Invalid authenticated user identity.");

                request.UserId = authenticatedUserId;
            }
            else
            {
                // Guests are always treated as anonymous checkout.
                request.UserId = null;
            }

            if (!ModelState.IsValid)
            {
                var messages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest($"Invalid input: {string.Join(", ", messages)}");
            }

            var result = await _orderService.PlaceOrderAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            _logger.LogInformation("Order {OrderId} placed successfully", result.Value!.Id);

            var kafkaPayload = JsonSerializer.Serialize(result.Value);
            _ = _kafkaProducerService.ProduceAsync("order-placed", result.Value.Id.ToString(), kafkaPayload);

            return Ok(result.Value);
        }

        /// <summary>Gets an order by ID (user view).</summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(long id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest("Invalid order ID.");

            var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}
