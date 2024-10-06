using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class BillDetail
    {
        public Guid Id { get; set; }

        [ForeignKey("ProductDetail")]
        public Guid ProductDetailId { get; set; }

        [ForeignKey("Bill")]
        public Guid BillId { get; set; }

        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        public Bill Bill { get; set; }
        public ProductDetail ProductDetail { get; set; }
    }
}
