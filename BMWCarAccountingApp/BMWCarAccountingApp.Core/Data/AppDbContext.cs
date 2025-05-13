using BMWCarAccountingApp.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace BMWCarAccountingApp.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().HasKey(c => c.Id);
            modelBuilder.Entity<Car>().Property(c => c.VIN).IsRequired().HasMaxLength(17);
            modelBuilder.Entity<Car>().Property(c => c.Model).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Car>().Property(c => c.Color).HasMaxLength(30);

            // Seed initial data
            modelBuilder.Entity<Car>().HasData(
                new Car
                {
                    Id = 1,
                    Model = "X5",
                    Year = 2020,
                    VIN = "WBACV6100L9C12345",
                    Color = "Black",
                    Price = 60000.00m,
                    DateAdded = DateTime.Now
                },
                new Car
                {
                    Id = 2,
                    Model = "M3",
                    Year = 2022,
                    VIN = "WBA5M2C58N7B67890",
                    Color = "Blue",
                    Price = 75000.00m,
                    DateAdded = DateTime.Now
                }
            );
        }
        
        public void Seed()
        {
            if (!Cars.Any())
            {
                Cars.AddRange(
                    new Car
                    {
                        Id = 1,
                        Model = "X5",
                        Year = 2020,
                        VIN = "WBACV6100L9C12345",
                        Color = "Black",
                        Price = 60000.00m,
                        DateAdded = DateTime.Now
                    },
                    new Car
                    {
                        Id = 2,
                        Model = "M3",
                        Year = 2022,
                        VIN = "WBA5M2C58N7B67890",
                        Color = "Blue",
                        Price = 75000.00m,
                        DateAdded = DateTime.Now
                    }
                );
                SaveChanges();
            }
        }
    }
}
