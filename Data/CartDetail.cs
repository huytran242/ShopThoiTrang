using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CartDetail
    {
        public Guid Id { get; set; }

        [ForeignKey("ProductDetail")]
        public Guid ProductDetailId { get; set; }

        [ForeignKey("Cart")]
        public Guid CartId { get; set; }

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        public Cart Cart { get; set; }
        public ProductDetail ProductDetail { get; set; }
    }
}
