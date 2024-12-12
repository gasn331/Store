using Store.Application.DTOs;
using Store.Domain.Entities;
using Store.Domain.Exceptions;
using Store.Domain.Interfaces;
using Store.Shared.Filters;

namespace Store.Application.Services
{
    public class OrderService
    {
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;

        // Constructor to initialize the repositories
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        // Creates a new order and returns its ID
        public async Task<Guid> CreateOrderAsync()
        {
            var order = new Order();
            await _orderRepository.AddAsync(order);
            return order.Id;
        }

        // Closes an existing order by its ID
        public async Task CloseOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new OrderException("Order not found");

            order.CloseOrder();

            await _orderRepository.UpdateAsync(order);
        }

        // Retrieves order details by ID and returns a DTO
        public async Task<OrderDto> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new OrderException("Order not found");

            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                IsClosed = order.IsClosed,
                TotalAmount = order.TotalAmount,
                Products = order.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList()
            };
        }

        // Retrieves a paginated list of orders based on filters
        public async Task<PagedResult<OrderDto>> GetOrdersAsync(OrderFilter filter, int currentPage, int pageSize)
        {
            List<Order> orders;
            int totalCount;
            
            (orders, totalCount) = await _orderRepository.GetOrdersAsync(filter, currentPage, pageSize);

            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                IsClosed = o.IsClosed,
                TotalAmount = o.TotalAmount,
                Products = o.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price
                }).ToList()
            }).ToList();

            return new PagedResult<OrderDto>
            {
                Items = ordersDto,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        // Adds a product to an order by its ID
        public async Task AddProductAsync(Guid orderId, ProductDto productDto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new OrderException("Order not found");


            var product = await _productRepository.GetByIdAsync(productDto.Id);

            if (product == null)
            {
                product = new Product(productDto.Id, productDto.Name, productDto.Price);

                await _productRepository.AddAsync(product);
            }

            order.AddProduct(product);


            await _orderRepository.UpdateAsync(order);
        }

        // Removes a product from an order by its product ID
        public async Task RemoveProductAsync(Guid orderId, Guid productId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new OrderException("Order not found");

            order.RemoveProduct(productId);

            await _orderRepository.UpdateAsync(order);
        }
    }
}
