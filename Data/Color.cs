using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Color
    {
        public Guid Id { get; set; }
        public string Ma { get; set; }

        public string Ten { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime NgayCapNhat { get; set; }
        public string TrangThai { get; set; }

        public ICollection<ProductDetailColor> ProductDetailColors { get; set; }
    }
}
