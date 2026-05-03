using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Flowers.Queries
{
    // Declare this interface instead of using MediatR for simplicity
    public interface IFlowerGetAllActiveHandler<R>
    {
        Task<R> Handle(CancellationToken cancellationToken);
    }

    /// <summary>Contract for retrieving a paginated list of active flowers.</summary>
    public interface IFlowerGetAllActivePagedHandler<R>
    {
        Task<R> Handle(int page, int pageSize, CancellationToken cancellationToken = default);
    }

    public class FlowerGetAllActive : IFlowerGetAllActiveHandler<IEnumerable<FlowerResponseItem>>
    {
        private readonly IFlowerResponsitory _responitory;
        public FlowerGetAllActive(IFlowerResponsitory responitory)
        {
            _responitory = responitory;
        }
        public async Task<IEnumerable<FlowerResponseItem>> Handle(CancellationToken cancellationToken)
        {
            var flowers = await _responitory.GetActivatedFlowerAsync();
            var result = flowers.Select(f => new FlowerResponseItem
            {
                Id = f.Id,
                Name = f.FlowerName,
                Image = f.FlowerImageUrl,
                CategoryName = f.Category?.Name ?? "",
                UnitPrice = f.UnitPrice.Price.Amount,
                UnitCurrency = f.UnitPrice.Price.Currency,
            });
            return result;
        }
    }

    /// <summary>Returns a paginated page of active flowers with a total count.</summary>
    public class FlowerGetAllActivePaged : IFlowerGetAllActivePagedHandler<PagedResult<FlowerResponseItem>>
    {
        private readonly IFlowerResponsitory _responitory;

        public FlowerGetAllActivePaged(IFlowerResponsitory responitory)
        {
            _responitory = responitory;
        }

        /// <summary>Handles the paginated active-flower query.</summary>
        public async Task<PagedResult<FlowerResponseItem>> Handle(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var flowers = await _responitory.GetActivatedFlowerPagedAsync(page, pageSize, cancellationToken);
            var total = await _responitory.CountActivatedFlowersAsync(cancellationToken);

            return new PagedResult<FlowerResponseItem>
            {
                Items = flowers.Select(f => new FlowerResponseItem
                {
                    Id = f.Id,
                    Name = f.FlowerName,
                    Image = f.FlowerImageUrl,
                    CategoryName = f.Category?.Name ?? "",
                    UnitPrice = f.UnitPrice.Price.Amount,
                    UnitCurrency = f.UnitPrice.Price.Currency,
                }),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    public interface IFlowerGetByIds<R>
    {
        Task<R> Handle(IEnumerable<long> flowerIds);
    }
    public class FlowerGetByIds : IFlowerGetByIds<IEnumerable<FlowerResponseItem>>
    {
        private readonly IFlowerResponsitory _responsitory;
        public FlowerGetByIds(IFlowerResponsitory responsitory)
        {
            _responsitory = responsitory;
        }
        public async Task<IEnumerable<FlowerResponseItem>> Handle(IEnumerable<long> flowerIds)
        {
            var flowers = await _responsitory.GetByIdsAsync(flowerIds);
            var result = flowers.Select(f => new FlowerResponseItem
            {
                Id = f.Id,
                Name = f.FlowerName,
                Image = f.FlowerImageUrl,
                UnitPrice = f.UnitPrice.Price.Amount,
                UnitCurrency = f.UnitPrice.Price.Currency,
            });
            return result;
        }
    }

    public interface IFlowerSearch<R>
    {
        Task<R> Handle(string keyword);
    }
    public class FlowerSearch : IFlowerSearch<IEnumerable<FlowerResponseItem>>
    {
        private readonly IFlowerResponsitory _responsitory;
        public FlowerSearch(IFlowerResponsitory responsitory)
        {
            _responsitory = responsitory;
        }
        public async Task<IEnumerable<FlowerResponseItem>> Handle(string keyword)
        {
            var flowers = await _responsitory.SearchAsync(keyword);
            var result = flowers.Select(f => new FlowerResponseItem
            {
                Id = f.Id,
                Name = f.FlowerName,
                Image = f.FlowerImageUrl,
                UnitPrice = f.UnitPrice.Price.Amount,
                UnitCurrency = f.UnitPrice.Price.Currency,
            });
            return result;
        }
    }
    /// <summary>
    /// Handler for getting a single flower by its ID, returning full detail including description and stock.
    /// </summary>
    public interface IFlowerGetByIdHandler<R>
    {
        Task<R> Handle(long id, CancellationToken cancellationToken = default);
    }
    /// <summary>
    /// Retrieves detailed information for a single flower by ID.
    /// Returns null when the flower does not exist.
    /// </summary>
    public class FlowerGetById : IFlowerGetByIdHandler<FlowerDetailResponseItem?>
    {
        private readonly IFlowerResponsitory _responsitory;
        public FlowerGetById(IFlowerResponsitory responsitory)
        {
            _responsitory = responsitory;
        }
        public async Task<FlowerDetailResponseItem?> Handle(long id, CancellationToken cancellationToken = default)
        {
            var flower = await _responsitory.GetByIdAsync(id);
            if (flower == null) return null;
            return new FlowerDetailResponseItem
            {
                Id = flower.Id,
                Name = flower.FlowerName,
                Image = flower.FlowerImageUrl,
                CategoryName = flower.Category?.Name ?? "",
                UnitPrice = flower.UnitPrice.Price.Amount,
                UnitCurrency = flower.UnitPrice.Price.Currency,
                Description = flower.FlowerDescription,
                StockQuantity = flower.Stock?.Quantity ?? 0,
                QuantityUnit = flower.Stock?.QuantityUnit.ToString(),
            };
        }
    }

    public interface IFlowerGetAllHandler<R>
    {
        Task<R> Handle(CancellationToken cancellationToken);
    }
    public class FlowerGetAll : IFlowerGetAllHandler<IEnumerable<FlowerAdminResponse>>
    {
        private readonly IFlowerResponsitory _responitory;
        public FlowerGetAll(IFlowerResponsitory responitory)
        {
            _responitory = responitory;
        }
        public async Task<IEnumerable<FlowerAdminResponse>> Handle(CancellationToken cancellationToken)
        {
            var flowers = await _responitory.GetAllAsync();
            var result = flowers.Select(f => new FlowerAdminResponse
            {
                Id = f.Id,
                Name = f.FlowerName,
                Image = f.FlowerImageUrl,
                CategoryName = f.Category?.Name ?? "",
                UnitPrice = f.UnitPrice.Price.Amount,
                UnitCurrency = f.UnitPrice.Price.Currency,
                Status = f.IsActive,
            });
            return result;
        }
    }

    /// <summary>
    /// Handler interface for validating a shopping cart against current stock and active status.
    /// </summary>
    public interface IFlowerValidateCartHandler<R>
    {
        Task<R> Handle(CartValidationRequestDto request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Validates each item in the cart: checks that the flower exists, is active,
    /// and has sufficient stock for the requested quantity.
    /// </summary>
    public class FlowerValidateCart : IFlowerValidateCartHandler<CartValidationResponseDto>
    {
        private readonly IFlowerResponsitory _responsitory;
        public FlowerValidateCart(IFlowerResponsitory responsitory)
        {
            _responsitory = responsitory;
        }

        /// <summary>
        /// Returns a per-item validation result. See <see cref="CartValidationStatus"/> for status values.
        /// </summary>
        public async Task<CartValidationResponseDto> Handle(CartValidationRequestDto request, CancellationToken cancellationToken = default)
        {
            var flowerIds = request.Items.Select(i => i.FlowerId).Distinct();
            var flowers = (await _responsitory.GetByIdsWithStockAsync(flowerIds))
                .ToDictionary(f => f.Id);

            var results = request.Items.Select(item =>
            {
                if (!flowers.TryGetValue(item.FlowerId, out var flower))
                {
                    return new CartItemValidationResult
                    {
                        FlowerId = item.FlowerId,
                        RequestedQuantity = item.Quantity,
                        Status = CartValidationStatus.NotFound
                    };
                }

                if (!flower.IsActive)
                {
                    return new CartItemValidationResult
                    {
                        FlowerId = item.FlowerId,
                        RequestedQuantity = item.Quantity,
                        Status = CartValidationStatus.Inactive
                    };
                }

                var availableQty = flower.Stock?.Quantity ?? 0;
                return new CartItemValidationResult
                {
                    FlowerId = item.FlowerId,
                    RequestedQuantity = item.Quantity,
                    Status = availableQty >= item.Quantity ? CartValidationStatus.Available : CartValidationStatus.OutOfStock
                };
            }).ToList();

            return new CartValidationResponseDto { Results = results };
        }
    }
}
