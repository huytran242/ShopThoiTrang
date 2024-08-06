namespace WebThoiTrang.Models
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string img { get; set; }
        public string CategoryName { get; set; } 
        public int Quantity { get; set; }
        public decimal TotalRevenue { get; set; }

        public int Stock { get; set; }
        public string ? Description { get; set; }


    }
}
