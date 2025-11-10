using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Entities;

namespace FlowerShop.Application.Interfaces
{
    public interface IFlowerService
    {
        Task<IEnumerable<FlowerResponseItem>> GetAllActiveFlowersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FlowerAdminResponse>> GetAllFlowersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<FlowerResponseItem>> SearchFlowersAsync(string keyword);
        Task<FlowerResponseItem> CreateFlowersAsync(CreateFlowerDto request);
        Task<bool> UpdateFlowerAsync(UpdateFlowerDto request);
        Task<Flower?> UpdateFlowerStatusAsync((long, bool) request);
        Task<bool> DeleteFlowerAsync(long flowerId);
    }
}
