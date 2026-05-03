using FlowerShop.Api.ExceptionHandlers;
using FlowerShop.Api.Extensions;
using FlowerShop.Application.Dtos.ChatFeature;
using FlowerShop.Infrastructure.AIServices;
using FlowerShop.Infrastructure.Configurations;
using FlowerShop.Infrastructure.VectorDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.AddAspireSqlServer();
builder.AddAspireBlobStorage();

builder.AddServiceDefaults();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/restaurantbooking-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var frontendBaseUrl = builder.Configuration["FrontendBaseUrl"] ?? "http://localhost:5173";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("allowVueSpa",
        builder =>
        {
            builder.WithOrigins(frontendBaseUrl)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true);
        });
});
builder.Services.AddControllers();

// Use Microsoft Agent Fx
builder.AddAiAgents(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("sql")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'sql' or 'DefaultConnection' not found.");
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

// Register JWT and SMTP configuration
builder.Services.AddAuthInfrastructure(
    jwtOptions => builder.Configuration.GetSection("Jwt").Bind(jwtOptions),
    smtpOptions => builder.Configuration.GetSection("Smtp").Bind(smtpOptions));

// Configure JWT Bearer authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter<AIChatRole>(JsonNamingPolicy.CamelCase)));

// builder.Services.AddOptions<GoogleTextSearchSettings>()
//                .Bind(builder.Configuration.GetSection("GoogleTextSearchSettings"));

// Add ChromaDB Client
builder.Services.AddChromaDb(builder.Configuration);

builder.Services.AddEmbeddingGenerator(builder.Configuration);

builder.Services.AddSingleton<IVectorDbContext, VectorDbContext>();

builder.AddKafkaServices();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

await app.ApplyMigrationsAsync();
await app.SeedAdminUserAsync();
await app.EnsureBlobContainerExistsAsync("flower-images");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //https://localhost:7204/swagger/index.html
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("allowVueSpa");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

