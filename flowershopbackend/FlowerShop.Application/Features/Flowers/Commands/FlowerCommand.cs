using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Entities;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Domain.ValueObject;

namespace FlowerShop.Application.Features.Flowers.Commands
{
    public interface IFlowerCreateCommand<I, R>
    {
        Task<R> Handle(I request, CancellationToken cancellationToken);
    }
    public class FlowerCreateCommand : IFlowerCreateCommand<CreateFlowerDto, FlowerResponseItem>
    {
        private readonly IFlowerResponsitory _responsitory;
        private readonly IUnitOfWork _unitOfWork;
        public FlowerCreateCommand(IFlowerResponsitory responsitory, IUnitOfWork unitOfWork)
        {
            _responsitory = responsitory;
            _unitOfWork = unitOfWork;
        }
        public async Task<FlowerResponseItem> Handle(CreateFlowerDto request, CancellationToken cancellationToken)
        {
            var newPricing = new FlowerPrice(request.UnitPrice, request.UnitCurrency);
            var flower = new Flower
            {
                FlowerName = request.Name,
                FlowerImageUrl = request.ImageUrl,
                FlowerDescription = request.Description,
                UnitPrice = new FlowerPricing
                {
                    Price = newPricing
                },
                CategoryId = request.CategoryId
            };

            var createdFlower =  await _responsitory.AddAsync(flower);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new FlowerResponseItem
            {
                Id = createdFlower.Id,
                Name = createdFlower.FlowerName,
                Image = createdFlower.FlowerImageUrl,
                UnitPrice = createdFlower.UnitPrice.Price.Amount,
                UnitCurrency = createdFlower.UnitPrice.Price.Currency,
            };
        }
    }

    public interface IFlowerUpdateCommand<I, R>
    {
        Task<R> Handle(I request, CancellationToken cancellationToken);
    }
    public class FlowerUpdateCommand : IFlowerUpdateCommand<UpdateFlowerDto, bool>
    {
        private readonly IFlowerResponsitory _responsitory;
        private readonly IUnitOfWork _unitOfWork;
        public FlowerUpdateCommand(IFlowerResponsitory responsitory, IUnitOfWork unitOfWork)
        {
            _responsitory = responsitory;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(UpdateFlowerDto request, CancellationToken cancellationToken)
        {
            var existingFlower = await _responsitory.GetByIdAsync(request.Id);
            if (existingFlower == null)
            {
                return false;
            }
            existingFlower.FlowerImageUrl = request.ImageUrl;
            if (existingFlower.UnitPrice != null)
            {
                existingFlower.UnitPrice.UpdatePrice(request.UnitPrice, request.UnitCurrency);
                // In real case, we should set the effective date based on business demands
                existingFlower.UnitPrice.UpdateEffectiveDate(DateTime.UtcNow, DateTime.UtcNow);
            }
            await _responsitory.UpdateAsync(existingFlower);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
    public interface IFlowerDeleteCommand<I, R>
    {
        Task<R> Handle(I request, CancellationToken cancellationToken);
    }
    public class FlowerDeleteCommand : IFlowerDeleteCommand<long, bool>
    {
        private readonly IFlowerResponsitory _responsitory;
        private readonly IUnitOfWork _unitOfWork;
        public FlowerDeleteCommand(IFlowerResponsitory responsitory, IUnitOfWork unitOfWork)
        {
            _responsitory = responsitory;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(long request, CancellationToken cancellationToken)
        {
            var existingFlower = await _responsitory.GetByIdAsync(request);
            if (existingFlower == null)
            {
                return false;
            }
            await _responsitory.DeleteAsync(request);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
    public interface IFlowerUpdateStatusCommand<I, R>
    {
        Task<R> Handle(I request, CancellationToken cancellationToken);
    }
    public class FlowerUpdateStatusCommand : IFlowerUpdateStatusCommand<(long, bool), Flower?>
    {
        private readonly IFlowerResponsitory _responsitory;
        private readonly IUnitOfWork _unitOfWork;
        public FlowerUpdateStatusCommand(IFlowerResponsitory responsitory, IUnitOfWork unitOfWork)
        {
            _responsitory = responsitory;
            _unitOfWork = unitOfWork;
        }
        public async Task<Flower?> Handle((long, bool) request, CancellationToken cancellationToken)
        {
            var existingFlower = await _responsitory.GetByIdAsync(request.Item1);
            if (existingFlower == null)
            {
                return null;
            }
            existingFlower.IsActive = request.Item2;
            await _responsitory.UpdateAsync(existingFlower);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return existingFlower;
        }
    }
}
