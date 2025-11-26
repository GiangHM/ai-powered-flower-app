using Confluent.Kafka;
using FlowerShop.Api.ExceptionHandlers;
using FlowerShop.Application.Dtos.ChatFeature;
using FlowerShop.Infrastructure.Agent;
using FlowerShop.Infrastructure.Agent.AgentPlugins;
using FlowerShop.Infrastructure.AIServices;
using FlowerShop.Infrastructure.Configurations;
using FlowerShop.Infrastructure.Persistence;
using FlowerShop.Infrastructure.VectorDb;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddAspireSqlServer();

builder.AddServiceDefaults();


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/restaurantbooking-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("allowVueSpa",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173/")
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true);
        });
});
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddApplication();
builder.Services.AddInfrastructure(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<AIChatRole>(JsonNamingPolicy.CamelCase)));

builder.Services.AddOptions<GoogleTextSearchSettings>()
                .Bind(builder.Configuration.GetSection("GoogleTextSearchSettings"));

// Add ChromaDB Client
builder.Services.AddChromaDb(builder.Configuration);

// Add Semantic Kernel Services
builder.Services.AddKernelChatCompletionServices(builder.Configuration);

builder.Services.AddKernelEmbedding(builder.Configuration);

builder.Services.AddSingleton<IVectorDbContext, VectorDbContext>();

builder.AddKafkaServices();

builder.Services.AddTransient<WriterAgent>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();


// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FlowerShopDbContext>();
    try
    {
        await context.Database.EnsureCreatedAsync();
        Log.Information("Database created and seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while creating the database");
    }
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    //https://localhost:7204/swagger/index.html
    app.MapOpenApi();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Flower API");
    });
}

app.UseHttpsRedirection();

app.UseCors("allowVueSpa");

app.UseAuthorization();

app.MapControllers();

app.Run();

