namespace Store.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public Product(string name, decimal price)
        {
            Id = Guid.NewGuid();
            Name = name;
            Price = price;
        }

        public Product(Guid id, string name, decimal price) 
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
}
