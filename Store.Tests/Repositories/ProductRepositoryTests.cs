using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Infrastructure.Data;
using Store.Infrastructure.Repositories;

namespace Store.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private AppDbContext _dbContext;
        private ProductRepository _productRepository;

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
            _productRepository = new ProductRepository(_dbContext);
        }

        [Test]
        public async Task AddAsync_Should_Add_Product()
        {
            // Arrange
            var product = new Product("Product 1", 100m);

            // Act
            await _productRepository.AddAsync(product);

            // Assert
            var addedProduct = await _dbContext.Products.FindAsync(product.Id);
            Assert.IsNotNull(addedProduct);
            Assert.That(addedProduct.Name, Is.EqualTo(product.Name));
            Assert.That(addedProduct.Price, Is.EqualTo(product.Price));
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Product_When_Found()
        {
            // Arrange
            var product = new Product("Product 1", 100m);
            await _productRepository.AddAsync(product);

            // Act
            var retrievedProduct = await _productRepository.GetByIdAsync(product.Id);

            // Assert
            Assert.IsNotNull(retrievedProduct);
            Assert.That(retrievedProduct.Id, Is.EqualTo(product.Id));
            Assert.That(retrievedProduct.Name, Is.EqualTo(product.Name));
            Assert.That(retrievedProduct.Price, Is.EqualTo(product.Price));
        }

        [Test]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            var nonExistentProductId = Guid.NewGuid();

            // Act
            var retrievedProduct = await _productRepository.GetByIdAsync(nonExistentProductId);

            // Assert
            Assert.IsNull(retrievedProduct);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted(); // Cleans up the database after the tests
            _dbContext.Dispose();
        }
    }
}
