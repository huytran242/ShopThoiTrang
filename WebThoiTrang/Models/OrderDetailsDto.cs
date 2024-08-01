using Data;

namespace WebThoiTrang.Models
{
    public class OrderDetailsDto
    {
        public LoginVM User { get; set; }
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } 
        public List<ProductDto> Products { get; set; }
    }
}
