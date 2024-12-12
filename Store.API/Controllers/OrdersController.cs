using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs;
using Store.Application.Services;
using Store.Domain.Exceptions;
using Store.Shared.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace Store.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [SwaggerTag("API for managing orders and associated products")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new order", Description = "Creates a new order and returns its Id.")]
        [SwaggerResponse(201, "Order created successfully", typeof(Guid))]
        public async Task<IActionResult> CreateOrder()
        {
            var id = await _orderService.CreateOrderAsync();

            return CreatedAtAction(nameof(GetOrder), new { orderId = id }, id);
        }

        [HttpPost("{orderId}/products")]
        [SwaggerOperation(Summary = "Add a product to an order", Description = "Adds a product to the specified order.")]
        [SwaggerResponse(204, "Product added successfully")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> AddProduct(Guid orderId, [FromBody] ProductDto productDto)
        {
            await _orderService.AddProductAsync(orderId, productDto);
            return NoContent();
        }

        [HttpDelete("{orderId}/products/{productId}")]
        [SwaggerOperation(Summary = "Remove a product from an order", Description = "Removes the specified product from the specified order.")]
        [SwaggerResponse(204, "Product removed successfully")]
        [SwaggerResponse(404, "Order or product not found")]
        [SwaggerResponse(400, "Invalid operation")]
        public async Task<IActionResult> RemoveProduct(Guid orderId, Guid productId)
        {
            try
            {
                await _orderService.RemoveProductAsync(orderId, productId);
                return NoContent();
            }
            catch(Exception ex)
            {
                if (ex is OrderException)
                    return NotFound(ex.Message);
                if (ex is InvalidOperationException)
                    return BadRequest(ex.Message);

                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{orderId}/close")]
        [SwaggerOperation(Summary = "Close an order", Description = "Closes the specified order and marks it as completed.")]
        [SwaggerResponse(204, "Order closed successfully")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> CloseOrder(Guid orderId)
        {
            await _orderService.CloseOrderAsync(orderId);
            return NoContent();
        }

        [HttpGet("{orderId}")]
        [SwaggerOperation(Summary = "Retrieve an order", Description = "Retrieves the details of the specified order.")]
        [SwaggerResponse(200, "Order retrieved successfully", typeof(OrderDto))]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if(order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieve a list of orders", Description = "Retrieves a paginated list of orders.")]
        [SwaggerResponse(200, "Orders retrieved successfully", typeof(PagedResult<OrderDto>))]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderFilter filter, int currentPage = 1, int pageSize = 10)
        {
            var result = await _orderService.GetOrdersAsync(filter, currentPage, pageSize);
            return Ok(result);
        }
    }
}
