namespace WebThoiTrang.Models
{
   
        public class OrdersListDto
        {
            public Guid OrderId { get; set; }
            public string Username { get; set; }
            public DateTime CreatedAt { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; }
            public string ? LyDoHuyDon {  get; set; }
            public List<ProductDto> Products { get; set; } = new List<ProductDto>();
        }

    
}
