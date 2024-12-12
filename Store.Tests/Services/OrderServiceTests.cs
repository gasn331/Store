using Moq;
using NUnit.Framework;
using Store.Application.Services;
using Store.Domain.Interfaces;
using Store.Domain.Entities;
using Store.Application.DTOs;
using Store.Domain.Exceptions;

namespace Store.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();

            // Creates OrderService Mock
            _orderService = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object);
        }

        [Test]
        public async Task AddProductAsync_Should_Add_Product_To_Order()
        {
            // Arrange
            var productDto = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 100.0m
            };

            var order = new Order();
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(order.Id))
                                .ReturnsAsync(order);

            // Act
            await _orderService.AddProductAsync(order.Id, productDto);

            // Assert
            _orderRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Order>(o => o.Products.Count == 1)), Times.Once);
        }

        [Test]
        public async Task AddProductAsync_Should_Throw_OrderNotFoundException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var productDto = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 100.0m
            };

            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(orderId))
                                .ReturnsAsync((Order)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<OrderException>(async () => await _orderService.AddProductAsync(orderId, productDto));
            Assert.That(ex.Message, Is.EqualTo("Order not found"));
        }
    }
}