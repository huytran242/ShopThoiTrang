﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ProductDetailColor
    {
        public Guid Id { get; set; }

        [ForeignKey("ProductDetail")]
        public Guid ProductDetailId { get; set; }

        [ForeignKey("Color")]
        public Guid ColorId { get; set; }

        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string TrangThai { get; set; }

        public ProductDetail ProductDetail { get; set; }
        public Color Color { get; set; }
    }
}