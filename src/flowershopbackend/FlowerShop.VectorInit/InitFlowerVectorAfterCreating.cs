using FlowerShop.Application.Dtos;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.VectorDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace FlowerShop.VectorInit;

public class InitFlowerVectorAfterCreating
{
    private readonly ILogger<InitFlowerVectorCollection> _logger;
    private readonly IVectorDbContext _vectorDbContext;

    public InitFlowerVectorAfterCreating(ILogger<InitFlowerVectorCollection> logger
        , IVectorDbContext vectorDbContext
        )
    {
        _logger = logger;
        _vectorDbContext = vectorDbContext;
    }

    [Function("InitFlowerVectorAfterCreating")]
    public async Task Run([KafkaTrigger("ConnectionStrings:kafka",
        "flower-vectors",
        Protocol = BrokerProtocol.NotSet,
        AuthenticationMode = BrokerAuthenticationMode.NotSet,
        ConsumerGroup = "$Default")] string eventData, FunctionContext context)
    {
        var eventJsonObject = JObject.Parse(eventData);

        var kafkavalue = eventJsonObject["Value"]?.ToString() ?? "";
        var inputModel = JsonSerializer.Deserialize<InitVectorDataRequest>(kafkavalue);

        if (inputModel == null || !inputModel.Payload.Any())
            return;

        if (inputModel.Action == "Upsert")
            await _vectorDbContext.InitMemoryContextAsync(inputModel.Payload);
        else
        {
            var deletionRequest = inputModel.Payload.Select(item => item.Id.ToString()).ToList();
            await _vectorDbContext.DeleteAsync(deletionRequest);
        }

        _logger.LogInformation("Vector DB initialization completed.");

    }
}