using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DSD603_Bike_Shop.Models;

namespace DSD603_BikeShopDB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DSD603_Bike_Shop.Models.Customer> Customer { get; set; } = default!;
        public DbSet<DSD603_Bike_Shop.Models.Order> Order { get; set; } = default!;
        public DbSet<DSD603_Bike_Shop.Models.Staff> Staff { get; set; } = default!;
        public DbSet<DSD603_Bike_Shop.Models.Stock> Stock { get; set; } = default!;
    }
}
