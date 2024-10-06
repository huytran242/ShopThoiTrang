using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Material
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }
        [Required]
        [MaxLength(50)]
        public string Ten { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string TrangThai { get; set; }
        public Product Product { get; set; }

    }
}
