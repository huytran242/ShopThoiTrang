namespace WebThoiTrang.Models
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public decimal TotalAmount { get; set; }
        public List<ProductDto> Products { get; set; }
        public string Status { get; set; }
    }
}
