using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.VectorDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FlowerShop.VectorInit;

public class InitFlowerVectorCollection
{
    private readonly ILogger<InitFlowerVectorCollection> _logger;
    private readonly IVectorDbContext _vectorDbContext;
    private IFlowerResponsitory _flowerService;

    public InitFlowerVectorCollection(ILogger<InitFlowerVectorCollection> logger
        , IVectorDbContext vectorDbContext
        , IFlowerResponsitory flowerService
        )
    {
        _logger = logger;
        _vectorDbContext = vectorDbContext;
        _flowerService = flowerService;
    }

    // This function will be triggered by an HTTP request.
    // It should be triggered manually to initialize the flower vector collection in the vector database once.
    [Function("FlowerVectorDataInit")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var triggerReason = await new StreamReader(req.Body).ReadToEndAsync();

        _logger.LogInformation("Init vector db context because of {TriggerReason}", triggerReason);

        var flowers = await _flowerService.GetAllAsync();
        var flowerDtos = flowers.Select(f => new FlowerShop.Application.Dtos.FlowerVectorDataRequestDto
        {
            Id = f.Id,
            Name = f.FlowerName,
            UnitPrice = f.UnitPrice.Price.Amount,
            UnitCurrency = f.UnitPrice.Price.Currency,
            Description = f.FlowerDescription,
        });
        await _vectorDbContext.InitMemoryContextAsync(flowerDtos);

        _logger.LogInformation("Function execution completed.");

        return new OkObjectResult("Vector DB initialization completed.");
    }
}