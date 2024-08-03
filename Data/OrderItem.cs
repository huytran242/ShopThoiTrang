using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
namespace Data
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }

        [Required(ErrorMessage = "Order is required.")]
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public Guid ProductId { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }
    }
}
