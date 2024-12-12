using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Domain.Interfaces;
using Store.Infrastructure.Data;
using Store.Shared.Filters;

namespace Store.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        // Constructor to initialize the DbContext
        public OrderRepository(AppDbContext context) 
        {
            _context = context;
        }

        // Adds a new order to the database
        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        // Retrieves an order by its ID, including related products
        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Retrieves orders based on filter criteria with pagination
        public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(OrderFilter filter, int currentPage, int pageSize)
        {

            if(currentPage < 1 || pageSize < 1)
            {
                throw new ArgumentException("CurrentPage and PageSize should be positive numbers.");
            }

            int skip = (currentPage - 1) * pageSize;

            var query = _context.Orders.Include(o => o.Products).AsQueryable();

            if(filter.IsClosed.HasValue)
            {
                query = query.Where(o => o.IsClosed == filter.IsClosed.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= filter.EndDate.Value);
            }

            int totalCount = await query.CountAsync();

            var orders =  await query.Skip(skip).Take(pageSize).ToListAsync();

            return (orders, totalCount);
        }

        // Updates an existing order in the database
        public async Task UpdateAsync(Order order)
        {
            var existingOrder = await _context.Orders
                                      .FirstOrDefaultAsync(o => o.Id == order.Id);

            if (existingOrder == null)
            {
                throw new InvalidOperationException("Order not found for update.");
            }

            // Ensure the entity is being tracked before updating
            if (_context.Entry(order).State == EntityState.Detached)
            {
                _context.Orders.Update(order);
            }

            await _context.SaveChangesAsync();
        }
    }
}
