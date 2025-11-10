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

        }
    }
}
