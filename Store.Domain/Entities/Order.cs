using Store.Domain.Exceptions;

namespace Store.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsClosed { get; private set; }
        public Decimal TotalAmount { get; private set; } = 0;
        public List<Product> Products { get; private set; } = new();

        public Order()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsClosed = false;
        }

        public void AddProduct(Product product)
        {
            if (IsClosed) 
                throw new OrderException("Cannot add products to a closed order");

            Products.Add(product);
            TotalAmount += product.Price;
        }

        public void RemoveProduct(Guid productId)
        {
            if (IsClosed)
                throw new OrderException("Cannot remove products from a closed order");

            var product = Products.FirstOrDefault(p => p.Id == productId) ?? throw new OrderException("Product not found in the order");

            Products.Remove(product);
            TotalAmount -= product.Price;
        }

        public void CloseOrder()
        {
            if (Products.Count == 0)
                throw new OrderException("Cannot close orders with no products");

            IsClosed = true;
        }
    }
}
