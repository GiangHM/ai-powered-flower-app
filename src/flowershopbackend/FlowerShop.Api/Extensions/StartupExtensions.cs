using Azure.Storage.Blobs;
using FlowerShop.Domain.Entities;
using FlowerShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace FlowerShop.Api.Extensions;

internal static class StartupExtensions
{
    /// <summary>
    /// Applies any pending EF Core migrations on startup.
    /// If the database already contains tables but is missing the <c>__EFMigrationsHistory</c>
    /// table (e.g. the schema was created outside of EF tooling), the history table is created
    /// and each migration whose sentinel table already exists is recorded as applied, so that
    /// <see cref="RelationalDatabaseFacadeExtensions.MigrateAsync"/> does not attempt to
    /// re-create existing objects.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FlowerShopDbContext>();
        try
        {
            await context.BaselineMigrationsHistoryAsync();

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                Log.Information("Applying pending migrations: {Migrations}", string.Join(", ", pendingMigrations));
            }

            await context.Database.MigrateAsync();
            Log.Information("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while applying database migrations");
            throw;
        }
    }

    /// <summary>
    /// Ensures the <paramref name="containerName"/> blob container exists, creating it if absent.
    /// Errors are logged but do not prevent startup.
    /// </summary>
    public static async Task EnsureBlobContainerExistsAsync(this WebApplication app, string containerName)
    {
        using var scope = app.Services.CreateScope();
        var blobServiceClient = scope.ServiceProvider.GetRequiredService<BlobServiceClient>();
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            Log.Information("Blob container '{ContainerName}' is ready", containerName);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while creating the '{ContainerName}' blob container", containerName);
        }
    }

    /// <summary>
    /// Creates <c>__EFMigrationsHistory</c> if absent, then inserts baseline rows for any
    /// migration whose sentinel table already exists in the database.
    /// </summary>
    private static async Task BaselineMigrationsHistoryAsync(this FlowerShopDbContext context)
    {
        // Create the history table if it does not exist yet.
#pragma warning disable EF1002 // SQL strings are fully hardcoded — no user input involved.
        await context.Database.ExecuteSqlRawAsync(
            """
            IF NOT EXISTS (
                SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '__EFMigrationsHistory'
            )
            BEGIN
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId]    nvarchar(150) NOT NULL,
                    [ProductVersion] nvarchar(32)  NOT NULL,
                    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                );
            END
            """);

        var rawVersion = typeof(FlowerShopDbContext).Assembly
            .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()?.InformationalVersion ?? "10.0.0";

        // ProductVersion column is nvarchar(32) — truncate to fit.
        var productVersion = rawVersion.Length > 32 ? rawVersion[..32] : rawVersion;

        // For each migration, insert a history row only when its sentinel table already exists
        // and the migration has not yet been recorded.
        var baselineMigrations = new[]
        {
            ("20260330141525_InitialCreate",   "FlowerCategories"),
            ("20260405052149_AddCustomerAuth", "Customers"),
            ("20260405072800_AddOrderTables",  "Orders"),
        };

        foreach (var (migrationId, sentinelTable) in baselineMigrations)
        {
            await context.Database.ExecuteSqlRawAsync(
                $"""
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = '{sentinelTable}'
                )
                AND NOT EXISTS (
                    SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '{migrationId}'
                )
                BEGIN
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                    VALUES ('{migrationId}', '{productVersion}');
                END
                """);
        }

        // Baseline AddOrderDeliveryInfo when Orders.DeliveryName already exists.
        await context.Database.ExecuteSqlRawAsync(
            $"""
            IF EXISTS (
                SELECT 1
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Orders' AND COLUMN_NAME = 'DeliveryName'
            )
            AND NOT EXISTS (
                SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260418121000_AddOrderDeliveryInfo'
            )
            BEGIN
                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES ('20260418121000_AddOrderDeliveryInfo', '{productVersion}');
            END
            """);

        // Baseline AddCustomerRole when Customers.Role already exists (pre-rename).
        await context.Database.ExecuteSqlRawAsync(
            $"""
            IF EXISTS (
                SELECT 1
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Customers' AND COLUMN_NAME = 'Role'
            )
            AND NOT EXISTS (
                SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260423010800_AddCustomerRole'
            )
            BEGIN
                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES ('20260423010800_AddCustomerRole', '{productVersion}');
            END
            """);
#pragma warning restore EF1002
    }

    /// <summary>
    /// Seeds the default admin user if no admin account exists.
    /// Admin credentials are read from configuration keys <c>AdminSeed:Email</c>,
    /// <c>AdminSeed:Password</c>, and <c>AdminSeed:Name</c>. Falls back to safe defaults.
    /// </summary>
    public static async Task SeedAdminUserAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FlowerShopDbContext>();

        try
        {
            var config = app.Configuration;
            var adminEmail = config["AdminSeed:Email"] ?? "admin@flowershop.local";
            var adminPassword = config["AdminSeed:Password"] ?? "Admin@123!";
            var adminName = config["AdminSeed:Name"] ?? "Admin";

            var existing = await context.Users
                .FirstOrDefaultAsync(u => u.Role == "Admin");

            if (existing is null)
            {
                var saltBytes = RandomNumberGenerator.GetBytes(32);
                var salt = Convert.ToBase64String(saltBytes);
                var hash = Convert.ToBase64String(
                    SHA256.HashData(Encoding.UTF8.GetBytes(adminPassword + salt)));

                var admin = new User
                {
                    Name = adminName,
                    Email = adminEmail,
                    Phone = "0000000000",
                    PasswordHash = $"{salt}:{hash}",
                    Role = "Admin",
                    Status = UserStatus.Active,
                    EmailVerified = true,
                    CreationDate = DateTime.UtcNow
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
                Log.Information("Admin user '{Email}' seeded successfully", adminEmail);
            }
            else
            {
                Log.Information("Admin user already exists, skipping seed");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the admin user");
        }
    }
}
