using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }

        [Required]
        [MaxLength(50)]
        public string Ten { get; set; }

        public string Anh { get; set; }
        public string GioiTinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string MatKhau { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string TrangThai { get; set; }

        public Cart Carts { get; set; }
        public ICollection<Bill> Bills { get; set; }
    }
}
