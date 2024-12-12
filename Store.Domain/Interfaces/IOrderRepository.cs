using Store.Domain.Entities;
using Store.Shared.Filters;

namespace Store.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order> GetByIdAsync(Guid orderId);
        Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(OrderFilter filter, int currentPage, int pageSize);
        Task UpdateAsync(Order order);
    }
}
