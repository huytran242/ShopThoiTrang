using Data;
using WebThoiTrang.Interface;
using WebThoiTrang.Models;

namespace WebThoiTrang.Service
{
    public class ProductService : IProductService
    {
        private readonly DbContextShop _context;

        public ProductService(DbContextShop context)
        {
            _context = context;
        }

        public ProductService()
        {
        }

        public Product GetProductById(Guid productId)
        {
            return _context.products.SingleOrDefault(p => p.ProductId == productId);
        }

        public void UpdateStock(Guid productId, int quantity)
        {
            var product = _context.products.SingleOrDefault(p => p.ProductId == productId);
            if (product != null)
            {
                product.Stock = quantity;
                _context.SaveChanges();
            }
        }
        public bool CheckStock(Guid productId, int quantity)
        {
            var product = _context.products.Find(productId);
            if (product == null)
            {
                return false; // Hoặc xử lý lỗi khác nếu sản phẩm không tồn tại
            }

            return product.Stock >= quantity;
        }
        public int GetStock(Guid productId)
        {
            var product = _context.products.FirstOrDefault(p => p.ProductId == productId);
            return product?.Stock ?? 0;
        }

    }
}
