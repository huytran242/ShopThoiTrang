using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebThoiTrang.Controllers;
using WebThoiTrang.Models;

namespace WebThoiTrang.Service
{
    public class CartService
    {
        // CartService.cs
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartSessionKey";
        private readonly DbContextShop _dbContext;
        private readonly ILogger<CartService> _logger;
        public CartService(IHttpContextAccessor httpContextAccessor, DbContextShop dbContext, ILogger<CartService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _logger = logger;

        }
        private List<OrderItem> cartItems = new List<OrderItem>();
        public void AddToCart(OrderItem orderItem)
        {
            var cart = GetCart();

            // Tải dữ liệu sản phẩm đầy đủ
            if (orderItem.ProductId != Guid.Empty)
            {
                orderItem.Product = _dbContext.products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.ProductId == orderItem.ProductId);
            }

            cart.Add(orderItem);
            SaveCart(cart);
        }

        public void UpdateCart(OrderItem updatedItem)
        {
            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(item => item.ProductId == updatedItem.ProductId);
            if (existingItem != null)
            {
                // Cập nhật số lượng sản phẩm
                existingItem.Quantity = updatedItem.Quantity;

                // Nếu số lượng sản phẩm là 0 hoặc âm, loại bỏ sản phẩm khỏi giỏ hàng
                if (existingItem.Quantity <= 0)
                {
                    cart.Remove(existingItem);
                }
            }
            else
            {
                // Nếu sản phẩm không tồn tại trong giỏ hàng, thêm sản phẩm mới
                cart.Add(updatedItem);
            }

            // Lưu giỏ hàng vào phiên làm việc
            SaveCart(cart);
        }

        public List<OrderItem> GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<OrderItem>(); // Nếu giỏ hàng rỗng, trả về danh sách rỗng
            }

            var cart = JsonConvert.DeserializeObject<List<OrderItem>>(cartJson);

            // Debugging: Kiểm tra số lượng giỏ hàng
            Console.WriteLine("Cart Count: " + cart.Count);

            // Populating product details
            foreach (var item in cart)
            {
                // Kiểm tra nếu ProductId không null, tải dữ liệu sản phẩm từ cơ sở dữ liệu
                if (item.ProductId != Guid.Empty)
                {
                    item.Product = _dbContext.products
                        .Include(p => p.Category)
                        .FirstOrDefault(p => p.ProductId == item.ProductId);
                }
            }

            return cart;
        }








        public void SaveCart(List<OrderItem> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonConvert.SerializeObject(cart, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            // Debugging: Log or inspect the cart JSON
            Console.WriteLine("Saving Cart JSON: " + cartJson);

            session.SetString(CartSessionKey, cartJson);
        }


    }

}
