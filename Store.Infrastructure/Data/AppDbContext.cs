using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;

namespace Store.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
