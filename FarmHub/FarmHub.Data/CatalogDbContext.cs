using FarmHub.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FarmHub.Data
{
    public class CatalogDbContext : IdentityDbContext<AuthUser, AuthRole, int>
    {
        public CatalogDbContext() { }

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductPortion>()
                .HasKey(t => new { t.PortionId, t.ProductId });

            modelBuilder.Entity<ProductPortion>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductPortions)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductPortion>()
                .HasOne(pt => pt.Portion)
                .WithMany(t => t.ProductPortions)
                .HasForeignKey(pt => pt.PortionId);

            modelBuilder.Entity<Discount>()
                .HasOne(p => p.Product)
                .WithOne(d => d.Discount)
                .HasForeignKey<Product>(p => p.DiscountId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.ShippingAddress)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BasketProduct>()
                .HasKey(b => new { b.BasketId, b.ProductId });

            modelBuilder.Entity<BasketProduct>()
                .HasOne(pr => pr.Product)
                .WithMany(bp => bp.BasketProducts)
                .HasForeignKey(pr => pr.ProductId);

            modelBuilder.Entity<BasketProduct>()
                .HasOne(b => b.Basket)
                .WithMany(bp => bp.BasketProducts)
                .HasForeignKey(b => b.BasketId);
            
            modelBuilder.Entity<Tag>(builder =>
            {
                builder.HasIndex(b => b.Name).IsUnique();
                builder.HasMany(e => e.Products).WithMany(p => p.ProductTags);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<HarvestPeriod> HarvestPeriods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<FarmerAssociation> FarmerAssociations { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<Portion> Portions { get; set; }
        public DbSet<ProductPortion> ProductPortions { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketProduct> BasketProducts { get; set; }
        
        public DbSet<CardPayment> CardPayments { get; set; }
        
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthRole> AuthRoles { get; set; }
        
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<CarouselItem> CarouselItems { get; set; }
        
        public DbSet<ConfirmationEmail> ConfirmationEmails { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}