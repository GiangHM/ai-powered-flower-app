using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FlowerShop.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by EF Core tools (dotnet ef migrations) when the
/// Aspire host is not available. Reads the connection string from environment
/// variables or a local appsettings file so that migrations can be generated
/// and applied without starting the full application.
/// </summary>
public class FlowerShopDbContextFactory : IDesignTimeDbContextFactory<FlowerShopDbContext>
{
    /// <summary>
    /// Creates a <see cref="FlowerShopDbContext"/> configured for design-time use.
    /// </summary>
    /// <param name="args">Arguments passed by the EF Core tools (unused).</param>
    /// <returns>A new <see cref="FlowerShopDbContext"/> instance.</returns>
    public FlowerShopDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration["ConnectionStrings__sql"]
            ?? "Server=localhost;Database=FlowerShopDb;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<FlowerShopDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new FlowerShopDbContext(optionsBuilder.Options);
    }
}
