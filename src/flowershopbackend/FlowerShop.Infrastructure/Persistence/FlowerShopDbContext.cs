using FlowerShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Infrastructure.Persistence
{
    public class FlowerShopDbContext : DbContext
    {
        public FlowerShopDbContext(DbContextOptions<FlowerShopDbContext> options) : base(options)
        {
        }

        public DbSet<FlowerCategory> FlowerCategories { get; set; } = null!;
        public DbSet<Flower> Flowers { get; set; } = null!;
        public DbSet<FlowerPricing> FlowerPrices { get; set; } = null!;
        public DbSet<FlowerStock> FlowerStocks { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Config Flower model
            modelBuilder.Entity<Flower>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Category)
                    .WithMany(topic => topic.Flowers)
                    .HasForeignKey(c => c.CategoryId);

            });

            //Config FlowerCategory model
            modelBuilder.Entity<FlowerCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.HasMany(e => e.Flowers)
                    .WithOne(e => e.Category)
                    .HasForeignKey(e => e.CategoryId);
            });

            //Config FlowerPricing model
            modelBuilder.Entity<FlowerPricing>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.HasOne<Flower>()
                      .WithOne(e => e.UnitPrice)
                      .HasForeignKey<FlowerPricing>(p => p.FlowerId);

                entity.OwnsOne(e => e.Price, money =>
                {
                    money.Property(m => m.Amount)
                        .HasColumnName("UnitPrice")
                        .HasPrecision(18, 2);
                    money.Property(m => m.Currency)
                        .HasColumnName("UnitPriceCurrency")
                        .HasMaxLength(3);
                });

                entity.OwnsOne(e => e.PriceEffective, effect =>
                {
                    effect.Property(m => m.From)
                        .HasColumnName("FromDate");
                    effect.Property(m => m.To)
                        .HasColumnName("ToDate");
                });
            });

            //Config FlowerStock model
            modelBuilder.Entity<FlowerStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.ImportedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne<Flower>()
                      .WithOne(e => e.Stock)
                      .HasForeignKey<FlowerStock>(s => s.FlowerId);
            });

            // Config User model
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(320).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.DeliveryAddress).HasMaxLength(500);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasMany(e => e.VerificationTokens)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Config EmailVerificationToken model
            modelBuilder.Entity<EmailVerificationToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.TokenHash).HasMaxLength(512).IsRequired();
                entity.HasIndex(e => e.TokenHash);

                entity.HasOne(t => t.User)
                      .WithMany(u => u.VerificationTokens)
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //Config Order model
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DeliveryName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.DeliveryEmail).HasMaxLength(320).IsRequired();
                entity.Property(e => e.DeliveryPhone).HasMaxLength(50).IsRequired();
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Status).HasConversion<int>();

                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //Config OrderItem model
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.FlowerName).HasMaxLength(200);
            });
        }
    }
}
