using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        [Required]
        [MaxLength(50)]
        public string Ten { get; set; }
        public decimal GiaTri { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; }

        public ICollection<Bill> Bills { get; set; }
    }
}
