using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }

        [Required]
        [MaxLength(100)]
        public string Ten { get; set; }

        public decimal Gia { get; set; }
        public int NamSX { get; set; }
        public string MoTa { get; set; }

        public Guid ProductTypeId { get; set; }
        public Guid BrandId { get; set; }
        public Guid ManufacturerId { get; set; }
        public Guid MaterialId { get; set; }
        public Material Material { get; set; }
        public ProductType ProductType { get; set; }
        public Brand Brand { get; set; }
        public Manufacturer Manufacturer { get; set; }

        public ICollection<ProductDetail> ProductDetails { get; set; }

    }

}
