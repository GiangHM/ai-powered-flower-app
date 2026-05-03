using FlowerShop.Application.Common;
using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Orders.Commands;
using FlowerShop.Application.Features.Orders.Queries;
using FlowerShop.Application.Features.Users.Commands;
using FlowerShop.Application.Features.Users.Queries;
using FlowerShop.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Api.Controllers;

/// <summary>
/// Admin endpoints for user management.
/// All routes require the caller to hold the <c>Admin</c> role.
/// </summary>
[ApiController]
[Route("api/Admin")]
[Authorize(Roles = "Admin")]
public partial class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }
    #region User Management
    /// <summary>Returns a paginated list of users with an optional status filter.</summary>
    /// <param name="page">Page number (1-based, default 1).</param>
    /// <param name="pageSize">Items per page (default 20, max 100).</param>
    /// <param name="status">Optional status filter (Pending, Active, Inactive).</param>
    /// <param name="query">Handler injected by DI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("Users")]
    [ProducesResponseType(typeof(PagedResult<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromServices] IGetUsersPagedQuery<PagedResult<UserResponseDto>> query = null!,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        UserStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<UserStatus>(status, ignoreCase: true, out var parsed))
                return BadRequest($"Invalid status value '{status}'. Valid values: Pending, Active, Inactive.");
            statusFilter = parsed;
        }

        _logger.LogInformation("Admin: listing users page={Page} pageSize={PageSize} status={Status}", page, pageSize, status);
        var result = await query.Handle(page, pageSize, statusFilter, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates a user's account status (suspend or reactivate).</summary>
    /// <param name="id">User ID.</param>
    /// <param name="request">New status payload.</param>
    /// <param name="command">Handler injected by DI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("Users/{id:long}/status")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserStatus(
        long id,
        [FromBody] UpdateUserStatusDto request,
        [FromServices] IUpdateUserStatusCommand<UpdateUserStatusDto, Result<UserResponseDto>> command = null!,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var messages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(string.Join("; ", messages));
        }

        _logger.LogInformation("Admin: updating status of user {UserId} to {Status}", id, request.Status);
        var result = await command.Handle(id, request, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>Edits a user's basic details (name, phone, delivery address).</summary>
    /// <param name="id">User ID.</param>
    /// <param name="request">Updated details payload.</param>
    /// <param name="command">Handler injected by DI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("Users/{id:long}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        long id,
        [FromBody] UpdateUserDto request,
        [FromServices] IUpdateUserCommand<UpdateUserDto, Result<UserResponseDto>> command = null!,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var messages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(string.Join("; ", messages));
        }

        _logger.LogInformation("Admin: updating details of user {UserId}", id);
        var result = await command.Handle(id, request, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>Returns the full order history for a given user.</summary>
    /// <param name="id">User ID.</param>
    /// <param name="query">Handler injected by DI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("Users/{id:long}/orders")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserOrders(
        long id,
        [FromServices] IGetUserOrdersQuery<Result<IEnumerable<OrderResponseDto>>> query = null!,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Admin: fetching orders for user {UserId}", id);
        var result = await query.Handle(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }
#endregion

    #region Order Management
    /// <summary>Returns a paginated list of all orders with an optional status filter.</summary>
    /// <param name="page">Page number (1-based, default 1).</param>
    /// <param name="pageSize">Items per page (default 20, max 100).</param>
    /// <param name="status">Optional status filter (Pending, Confirmed, Shipped, Delivered, Cancelled).</param>
    /// <param name="query">Handler injected by DI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("Orders")]
    [ProducesResponseType(typeof(PagedResult<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromServices] IGetOrdersPagedQuery<PagedResult<OrderResponseDto>> query = null!,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        OrderStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out var parsed))
                return BadRequest($"Invalid status value '{status}'. Valid values: {string.Join(", ", Enum.GetNames<OrderStatus>())}.");
            statusFilter = parsed;
        }

        _logger.LogInformation("Admin: listing orders page={Page} pageSize={PageSize} status={Status}", page, pageSize, status);
        var result = await query.Handle(page, pageSize, statusFilter, cancellationToken);
        return Ok(result);
    }

    /// <summary>Updates the status of an existing order.</summary>
    /// <param name="id">Order ID.</param>
    /// <param name="request">New status payload.</param>
    /// <param name="command">Handler injected by DI.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("Orders/{id:long}/status")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus(
        long id,
        [FromBody] UpdateOrderStatusDto request,
        [FromServices] IUpdateOrderStatusCommand<UpdateOrderStatusDto, Result<OrderResponseDto>> command = null!,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var messages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest(string.Join("; ", messages));
        }

        _logger.LogInformation("Admin: updating status of order {OrderId} to {Status}", id, request.Status);
        var result = await command.Handle(id, request, cancellationToken);
        if (!result.IsSuccess)
            return result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }
#endregion
}
