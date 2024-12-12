using System.Net.Http.Headers;

namespace Store.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
