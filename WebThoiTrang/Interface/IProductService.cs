using Data;

namespace WebThoiTrang.Interface
{
    public interface IProductService
    {
        Product GetProductById(Guid productId);
        void UpdateStock(Guid productId, int quantity);
        bool CheckStock(Guid productId, int quantity);
        int GetStock(Guid productId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }
}
