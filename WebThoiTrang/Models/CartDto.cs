namespace WebThoiTrang.Models
{
    public class CartDto
    {
        public List<ProductDto> Products { get; set; }= new List<ProductDto>();
        public decimal TotalAmount { get; set; }
        public LoginVM User { get; set; }
    }
}
