﻿namespace WebThoiTrang.Models
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string img { get; set; }
        public string CategoryName { get; set; } // Lấy tên Category thay vì đối tượng Category
        public int Quantity { get; set; }
    }
}