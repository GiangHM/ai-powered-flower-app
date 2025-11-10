using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.AIServices;
using FlowerShop.Infrastructure.Persistence;
using FlowerShop.Infrastructure.VectorDb;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FlowerShop.Infrastructure.Configurations;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddAspireSqlServer();

builder.AddServiceDefaults();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Add ChromaDB Client
builder.Services.AddChromaDb(builder.Configuration);

// Add Semantic Kernel Embedding Generator
builder.Services.AddKernelEmbedding(builder.Configuration);

builder.Services.AddSingleton<IVectorDbContext, VectorDbContext>();

builder.Services.AddScoped<IFlowerResponsitory, FlowerRespository>();


builder.Build().Run();
