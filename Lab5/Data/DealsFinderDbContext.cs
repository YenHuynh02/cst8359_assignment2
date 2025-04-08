using Lab5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Lab5.Data
{
    public class DealsFinderDbContext : DbContext
    {
        public DealsFinderDbContext(DbContextOptions<DealsFinderDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FoodDeliveryService> FoodDeliveryServices { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Deal> Deals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<FoodDeliveryService>().ToTable("FoodDeliveryService");
            modelBuilder.Entity<Subscription>().HasKey(x => new { x.CustomerId, x.FoodDeliveryServiceId });

            // Relationship between Customer and Subscriptions
            modelBuilder.Entity<Subscription>().HasOne(x => x.Customer).WithMany(x => x.Subscriptions).HasForeignKey(x => x.CustomerId);
            
            // Relationship between FoodDeliveryService and Subscriptions
            modelBuilder.Entity<Subscription>().HasOne(x => x.FoodDeliveryService).WithMany(x => x.Subscriptions).HasForeignKey(x => x.FoodDeliveryServiceId);

            modelBuilder.Entity<Deal>()
                .HasKey(d => new { d.FoodDeliveryServiceId });
        }
    }       
}
