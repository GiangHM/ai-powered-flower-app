using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Entities;

namespace FlowerShop.Application.Interfaces
{
    public interface IFlowerService
    {
        Task<FlowerDetailResponseItem?> GetFlowerByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IEnumerable<FlowerResponseItem>> GetAllActiveFlowersAsync(CancellationToken cancellationToken = default);
        Task<PagedResult<FlowerResponseItem>> GetAllActiveFlowersPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<FlowerAdminResponse>> GetAllFlowersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FlowerResponseItem>> SearchFlowersAsync(string keyword);
        Task<CartValidationResponseDto> ValidateCartAsync(CartValidationRequestDto request, CancellationToken cancellationToken = default);
        Task<FlowerResponseItem> CreateFlowersAsync(CreateFlowerDto request);
        Task<bool> UpdateFlowerAsync(UpdateFlowerDto request);
        Task<Flower?> UpdateFlowerStatusAsync((long, bool) request);
        Task<bool> DeleteFlowerAsync(long flowerId);
    }
}
