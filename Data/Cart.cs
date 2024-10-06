using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Cart
    {
        public Guid Id { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string TrangThai { get; set; }

        public Customer Customers { get; set; }
        public ICollection<CartDetail> CartDetails { get; set; }
    }
}
