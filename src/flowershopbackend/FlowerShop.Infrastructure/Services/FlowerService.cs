using FlowerShop.Application.Dtos;
using FlowerShop.Application.Features.Flowers.Commands;
using FlowerShop.Application.Features.Flowers.Queries;
using FlowerShop.Application.Interfaces;
using FlowerShop.Domain.Entities;

namespace FlowerShop.Infrastructure.Services
{
    public class FlowerService : IFlowerService
    {
        private readonly IFlowerGetAllActiveHandler<IEnumerable<FlowerResponseItem>> _getAllActivedHandler;
        private readonly IFlowerGetAllActivePagedHandler<PagedResult<FlowerResponseItem>> _getAllActivePagedHandler;
        private readonly IFlowerGetAllHandler<IEnumerable<FlowerAdminResponse>> _getAllHandler;
        private readonly IFlowerCreateCommand<CreateFlowerDto, FlowerResponseItem> _flowerCreateHandler;
        private readonly IFlowerUpdateCommand<UpdateFlowerDto, bool> _updateFlowerHandler;
        private readonly IFlowerDeleteCommand<long, bool> _deleteFlowerHandler;
        private readonly IFlowerSearch<IEnumerable<FlowerResponseItem>> _searchFlowerHandler;
        private readonly IFlowerUpdateStatusCommand<(long, bool), Flower?> _updateStatusHandler;
        private readonly IFlowerGetByIdHandler<FlowerDetailResponseItem?> _getByIdHandler;
        private readonly IFlowerValidateCartHandler<CartValidationResponseDto> _validateCartHandler;

        public FlowerService(IFlowerGetAllActiveHandler<IEnumerable<FlowerResponseItem>> getAllActivatedHandler
            , IFlowerGetAllActivePagedHandler<PagedResult<FlowerResponseItem>> getAllActivePagedHandler
            , IFlowerGetAllHandler<IEnumerable<FlowerAdminResponse>> getAllHandler
            , IFlowerCreateCommand<CreateFlowerDto, FlowerResponseItem> flowerCreateCommand
            , IFlowerUpdateCommand<UpdateFlowerDto, bool> updateFlowerHandler
            , IFlowerDeleteCommand<long, bool> deleteFlowerHandler
            , IFlowerSearch<IEnumerable<FlowerResponseItem>> searchFlowerHandler
            , IFlowerUpdateStatusCommand<(long, bool), Flower?> updateStatusHandler
            , IFlowerGetByIdHandler<FlowerDetailResponseItem?> getByIdHandler
            , IFlowerValidateCartHandler<CartValidationResponseDto> validateCartHandler)
        {
            _getAllActivedHandler = getAllActivatedHandler;
            _getAllActivePagedHandler = getAllActivePagedHandler;
            _getAllHandler = getAllHandler;
            _flowerCreateHandler = flowerCreateCommand;
            _updateFlowerHandler = updateFlowerHandler;
            _deleteFlowerHandler = deleteFlowerHandler;
            _searchFlowerHandler = searchFlowerHandler;
            _updateStatusHandler = updateStatusHandler;
            _getByIdHandler = getByIdHandler;
            _validateCartHandler = validateCartHandler;
        }

        public async Task<FlowerResponseItem> CreateFlowersAsync(CreateFlowerDto request)
        {
            return await _flowerCreateHandler.Handle(request, CancellationToken.None);
        }
        public async Task<bool> UpdateFlowerAsync(UpdateFlowerDto request)
        {
            return await _updateFlowerHandler.Handle(request, CancellationToken.None);
        }
        public async Task<Flower?> UpdateFlowerStatusAsync((long, bool) request)
        {
            return await _updateStatusHandler.Handle(request, CancellationToken.None);
        }
        public async Task<bool> DeleteFlowerAsync(long flowerId)
        {
            return await _deleteFlowerHandler.Handle(flowerId, CancellationToken.None);
        }
        public async Task<IEnumerable<FlowerResponseItem>> GetAllActiveFlowersAsync(CancellationToken cancellationToken = default)
        {
            return await _getAllActivedHandler.Handle(cancellationToken);
        }

        public async Task<PagedResult<FlowerResponseItem>> GetAllActiveFlowersPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _getAllActivePagedHandler.Handle(page, pageSize, cancellationToken);
        }
        public async Task<IEnumerable<FlowerAdminResponse>> GetAllFlowersAsync(CancellationToken cancellationToken = default)
        {
            return await _getAllHandler.Handle(cancellationToken);
        }
        public async Task<IEnumerable<FlowerResponseItem>> SearchFlowersAsync(string keyword)
        {
            var filteredFlowers = await _searchFlowerHandler.Handle(keyword);
            return filteredFlowers;
        }

        public Task<FlowerDetailResponseItem?> GetFlowerByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return _getByIdHandler.Handle(id, cancellationToken);
        }

        /// <summary>
        /// Validates each cart item against current flower stock and active status.
        /// </summary>
        public Task<CartValidationResponseDto> ValidateCartAsync(CartValidationRequestDto request, CancellationToken cancellationToken = default)
        {
            return _validateCartHandler.Handle(request, cancellationToken);
        }
    }
}
