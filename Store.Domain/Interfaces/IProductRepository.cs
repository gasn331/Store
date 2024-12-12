using Store.Domain.Entities;

namespace Store.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<Product> GetByIdAsync(Guid productId);
    }
}
