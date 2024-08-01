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
<<<<<<< HEAD
        public string Status { get; set; } 
=======

>>>>>>> fa7bf7715d95be9f883530a630fdf38a38bd80a1
        public List<ProductDto> Products { get; set; }
    }
}
