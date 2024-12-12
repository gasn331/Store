using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Store.Domain.Entities;
using Store.Infrastructure.Data;
using Store.Infrastructure.Repositories;
using Store.Shared.Filters;

namespace Store.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        private AppDbContext _dbContext;
        private OrderRepository _orderRepository;

        [SetUp]
        public void Setup()
        {
            // Set up the in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Using a unique name for the in-memory database
                            .Options;

            // Create the DbContext with the in-memory database
            _dbContext = new AppDbContext(options);
            _dbContext.Database.EnsureCreated();

            // Create the repository
            _orderRepository = new OrderRepository(_dbContext);
        }

        [Test]
        public async Task AddAsync_Should_Add_Order()
        {
            // Arrange
            var order = new Order();
            var product = new Product("Product 1", 100m);
            order.AddProduct(product);

            // Act
            await _orderRepository.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Assert
            var addedOrder = await _dbContext.Orders.FindAsync(order.Id);
            Assert.NotNull(addedOrder);
            Assert.That(addedOrder.Products.Count, Is.EqualTo(1));
            Assert.That(addedOrder.TotalAmount, Is.EqualTo(100m));
        }

        [Test]
        public async Task AddMultipleProducts_Should_Calculate_TotalAmount_Correctly()
        {
            // Arrange
            var order = new Order();
            var product1 = new Product("Product 1", 100m);
            var product2 = new Product("Product 2", 150m);
            order.AddProduct(product1);
            order.AddProduct(product2);

            // Act
            await _orderRepository.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Assert
            var addedOrder = await _dbContext.Orders.FindAsync(order.Id);
            Assert.NotNull(addedOrder);
            Assert.That(addedOrder.Products.Count, Is.EqualTo(2));
            Assert.That(addedOrder.TotalAmount, Is.EqualTo(250m));
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Order_When_Found()
        {
            // Arrange
            var order = new Order();
            var product = new Product("Product 1", 100m);
            order.AddProduct(product);
            await _orderRepository.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // Act
            var fetchedOrder = await _orderRepository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(fetchedOrder);
            Assert.That(fetchedOrder.Id, Is.EqualTo(order.Id));
            Assert.That(fetchedOrder.TotalAmount, Is.EqualTo(100m));
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_When_Order_Not_Found()
        {
            // Act
            var fetchedOrder = await _orderRepository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(fetchedOrder);
        }

        [Test]
        public async Task GetOrdersAsync_Should_Throw_ArgumentException_When_PageSize_Or_CurrentPage_Is_Invalid()
        {
            // Arrange
            var filter = new OrderFilter();
            int invalidPage = -1;
            int pageSize = 10;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _orderRepository.GetOrdersAsync(filter, invalidPage, pageSize));
            Assert.That(ex.Message, Is.EqualTo("CurrentPage and PageSize should be positive numbers."));

            // Testing negative PageSize
            invalidPage = 1;
            pageSize = -10;

            ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _orderRepository.GetOrdersAsync(filter, invalidPage, pageSize));
            Assert.That(ex.Message, Is.EqualTo("CurrentPage and PageSize should be positive numbers."));
        }

        [Test]
        public async Task GetOrdersAsync_Should_Return_Orders_With_Correct_Pagination()
        {
            // Arrange
            var order1 = new Order();
            var product1 = new Product("Product 1", 100m);
            order1.AddProduct(product1);
            // To adjust the date, we use internal creation logic
            var order1CreatedAt = new DateTime(2024, 1, 1);
            typeof(Order).GetProperty("CreatedAt").SetValue(order1, order1CreatedAt);
            await _orderRepository.AddAsync(order1);

            var order2 = new Order();
            var product2 = new Product("Product 2", 200m);
            order2.AddProduct(product2);
            var order2CreatedAt = new DateTime(2024, 2, 1);
            typeof(Order).GetProperty("CreatedAt").SetValue(order2, order2CreatedAt);
            await _orderRepository.AddAsync(order2);

            var order3 = new Order();
            var product3 = new Product("Product 3", 150m);
            order3.AddProduct(product3);
            var order3CreatedAt = new DateTime(2024, 3, 1);
            typeof(Order).GetProperty("CreatedAt").SetValue(order3, order3CreatedAt);
            await _orderRepository.AddAsync(order3);

            await _dbContext.SaveChangesAsync();

            // Act
            var (ordersPage1, totalCount1) = await _orderRepository.GetOrdersAsync(new OrderFilter(), 1, 2); // Page 1
            var (ordersPage2, totalCount2) = await _orderRepository.GetOrdersAsync(new OrderFilter(), 2, 2); // Page 2

            // Assert
            Assert.That(ordersPage1.Count, Is.EqualTo(2)); // Page 1 should have 2 orders
            Assert.That(ordersPage2.Count, Is.EqualTo(1)); // Page 2 should have 1 order
            Assert.That(totalCount1, Is.EqualTo(3)); // The total number of orders in the database should be 3
            Assert.That(totalCount2, Is.EqualTo(3)); // The total number of orders in the database should be 3
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted(); // Cleans up the database after the tests
            _dbContext.Dispose();
        }
    }
}
