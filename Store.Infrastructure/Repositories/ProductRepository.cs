using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Domain.Interfaces;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        // Constructor to initialize the DbContext
        public ProductRepository(AppDbContext context) 
        {
            _context = context;
        }

        // Adds a new product to the database
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // Retrieves a product by its ID
        public async Task<Product> GetByIdAsync(Guid productId)
        {
            return await _context.Products.FindAsync(productId);
        }
    }
}
