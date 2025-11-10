using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Interfaces;

namespace FlowerShop.Application.Features.Flowers.Queries
{
    // Declare this interface instead of using MediatR for simplicity
    public interface IFlowerGetAllActiveHandler<R>
    {
        Task<R> Handle(CancellationToken cancellationToken);
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
                Status = f.IsActive
            });
            return result;
        }
    }
}
