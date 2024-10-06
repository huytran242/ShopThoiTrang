//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Diagnostics;
//using WebThoiTrang.Models;
//using Data;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using WebThoiTrang.Service;
//using Newtonsoft.Json;
//using System.Security.Claims;
//using WebThoiTrang.Extensions;
//using WebThoiTrang.Interface;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//namespace WebThoiTrang.Controllers
//{
//    public class HomeController : BaseController
//    {
//        private readonly CartService _cartService;
//        private readonly ILogger<HomeController> _logger;
//        private readonly DbContextShop _context;
//        public HomeController(ILogger<HomeController> logger, DbContextShop context, CartService cartService)
//        {
//            _cartService = cartService;
//            _logger = logger;
//            _context = context;

//        }
//        public ActionResult Logout()
//        {
//            HttpContext.Session.Clear();

//            // Redirect to the login page or home page
//            return RedirectToAction("IndexShop", "Home");
//        }
//        public async Task<IActionResult> x()
//        {
//            return View();
//        }

//        public async Task<IActionResult> IndexShop(string searchTerm)
//        {
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest"; // Nếu không có Username trong Session, gán giá trị là "Guest"
//            var products = string.IsNullOrWhiteSpace(searchTerm) ?
//           await _context.products.Include(p => p.Category).ToListAsync() :
//           await _context.products.Include(p => p.Category)
//                                  .Where(p => p.Name.Contains(searchTerm) ||
//                                              p.Description.Contains(searchTerm) ||
//                                              p.Category.Name.Contains(searchTerm))
//                                  .ToListAsync();

//            ViewData["SearchTerm"] = searchTerm;
//            ViewData["Username"] = username;
//            return View(products);
//        }
//       public async Task<IActionResult> Details(Guid id)
//        {
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest"; // Nếu không có Username trong Session, gán giá trị là "Guest"
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var product = await _context.products
//                .Include(p => p.Category)
//                .FirstOrDefaultAsync(m => m.ProductId == id);

//            if (product == null)
//            {
//                return NotFound();
//            }

//            return View(product);
//        }
//        public IActionResult UserCreate()
//        {
//            return View();
//        }

//        // POST: Users/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UserCreate([Bind("UserId,Username,Password,Email,CreatedAt")] User user)
//        {
//            HttpContext.Session.Clear();

           
           
//            if (!ModelState.IsValid)
//            {
//                var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
//                if (existingUser != null)
//                {
//                    ModelState.AddModelError("Email", "Email is already in use.");
//                    return View(user);
//                }
//                user.Orders = new List<Order>();
//                user.UserId = Guid.NewGuid();
//                _context.Add(user);
//                await _context.SaveChangesAsync();
//                return RedirectToAction("IndexShop");
//            }
//            return View(user);
//        }


//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//        // GET: Users/Edit/5
//        public async Task<IActionResult> UserEdit(int? id)
//        {
//            if (id == null || _context.users == null)
//            {
//                return NotFound();
//            }

//            var user = await _context.users.FindAsync(id);
//            if (user == null)
//            {
//                return NotFound();
//            }
//            return View(user);
//        }

//        // POST: Users/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> UserEdit(Guid id, [Bind("UserId,Username,Password,Email,CreatedAt")] User user)
//        {
//            if (id != user.UserId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(user);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!UserExists(user.UserId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction("IndexShop");
//            }
//            return View(user);
//        }
//        public async Task<IActionResult> UserDelete(Guid id)
//        {
//            if (id == null || _context.users == null)
//            {
//                return NotFound();
//            }

//            var user = await _context.users
//                .FirstOrDefaultAsync(m => m.UserId == id);
//            if (user == null)
//            {
//                return NotFound();
//            }

//            return View(user);
//        }

//        // POST: Users/Delete/5
//        [HttpPost, ActionName("UserDelete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(Guid id)
//        {
//            if (_context.users == null)
//            {
//                return Problem("Entity set 'DbContextShop.users'  is null.");
//            }
//            var user = await _context.users.FindAsync(id);
//            if (user != null)
//            {
//                _context.users.Remove(user);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction("IndexShop");
//        }

//        private bool UserExists(Guid id)
//        {
//            return (_context.users?.Any(e => e.UserId == id)).GetValueOrDefault();
//        }

      


//        public IActionResult AddToCart(Guid productId)
//        {
//            try
//            {
//                // Tìm sản phẩm theo productId và chuyển đổi thành ProductDto
//                var product = _context.products
//                    .Include(p => p.Category) // Bao gồm Category nhưng không sử dụng trong DTO
//                    .Where(p => p.ProductId == productId)
//                    .Select(p => new ProductDto
//                    {
//                        ProductId = p.ProductId,
//                        Name = p.Name,
//                        Price = p.Price,
//                        img = p.img,
//                        CategoryName = p.Category.Name, // Chỉ lấy tên Category
//                        Stock = p.Stock // Lấy số lượng tồn kho
//                    })
//                    .FirstOrDefault();

//                if (product == null)
//                {
//                    return NotFound();
//                }

//                if (product.Stock <= 0)
//                {
//                    TempData["ErrorMessage"] = "Sản phẩm đã hết hàng.";
//                    return RedirectToAction("IndexShop"); // Chuyển hướng đến trang sản phẩm
//                }

//                // Lấy giỏ hàng hiện tại
//                var cartItems = _cartService.GetCart();
//                var existingItem = cartItems.FirstOrDefault(ci => ci.ProductId == productId);

//                if (existingItem != null)
//                {
//                    // Nếu sản phẩm đã tồn tại, chỉ cập nhật số lượng
//                    existingItem.Quantity += 1; // Hoặc cập nhật số lượng từ request
//                    _cartService.UpdateCart(new OrderItem
//                    {
//                        ProductId = productId,
//                        Quantity = existingItem.Quantity,
//                        Price = product.Price
//                    });
//                    TempData["Message"] = "Sản phẩm đã tồn tại trong giỏ hàng. Số lượng đã được cập nhật.";
//                }
//                else
//                {
//                    // Nếu sản phẩm chưa tồn tại, thêm sản phẩm mới vào giỏ hàng
//                    var orderItem = new OrderItem
//                    {
//                        ProductId = productId,
//                        Quantity = 1, // Hoặc lấy từ request
//                        Price = product.Price
//                    };
//                    _cartService.AddToCart(orderItem);
//                    TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng.";
//                }

//                // Lưu cartItems vào TempData
//                    TempData["CartItems"] = Newtonsoft.Json.JsonConvert.SerializeObject(
//                    cartItems.Select(ci => new ProductDto
//                    {
//                        ProductId = ci.ProductId,
//                        Name = ci.Product.Name,
//                        Price = ci.Price,
//                        img = ci.Product.img,
//                        CategoryName = ci.Product.Category.Name,
//                        Quantity = ci.Quantity
//                    }).ToList(),
//                    new JsonSerializerSettings
//                    {
//                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//                    });

//                return RedirectToAction("CartIndex");
//            }
//            catch (Exception ex)
//            {
//                // Ghi log lỗi và trả về trang lỗi
//                _logger.LogError(ex, "Error adding product to cart");
//                return StatusCode(500, $"Internal server error: {ex.Message}");
//            }
//        }


//        public IActionResult CartIndex()
//        {
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest"; // Nếu không có Username trong Session, gán giá trị là "Guest"
//            try
//            {
//                var cartItems = _cartService.GetCart();
               
//                var cartItemsDto = cartItems?.Select(ci => new ProductDto
//                {
//                    ProductId = ci.ProductId,
//                    Name = ci.Product.Name,
//                    Price = ci.Price,
//                    img = ci.Product.img,
//                    CategoryName = ci.Product.Category.Name,
//                    Quantity = ci.Quantity,
//                    Stock = ci.Product.Stock

//                }).ToList() 

//                ?? new List<ProductDto>();
//                var stockData = cartItemsDto.ToDictionary(p => p.ProductId.ToString(), p => p.Stock);
//                ViewBag.StockData = stockData;

//                TempData["CartItems"] = JsonConvert.SerializeObject(cartItemsDto, new JsonSerializerSettings
//                {
//                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
//                });

//                return View(new CartDto { Products = cartItemsDto });
//            }
//            catch (Exception ex)
//            {

//                _logger.LogError(ex, "Error retrieving cart items");
//                return StatusCode(500, "Internal server error");
//            }

//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult UpdateCart([FromBody] OrderItem updatedItem)
//        {
//            try
//            {
//                // Cập nhật giỏ hàng
//                _cartService.UpdateCart(updatedItem);

//                // Trả về phản hồi thành công
//                return Json(new { success = true });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating cart");
//                // Trả về phản hồi lỗi
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public IActionResult RemoveFromCart(Guid productId)
//        {
//            var cart = _cartService.GetCart();
//            var item = cart.FirstOrDefault(i => i.ProductId == productId);
//            if (item != null)
//            {
//                cart.Remove(item);
//                _cartService.SaveCart(cart);
//            }
//            return RedirectToAction("CartIndex");
//        }

//        [HttpPost]
//        public IActionResult UpdateQuantity(Guid productId, int quantity)
//        {
//            var cart = _cartService.GetCart();
//            var item = cart.FirstOrDefault(i => i.ProductId == productId);
//            if (item != null)
//            {
//                item.Quantity = quantity;
//                _cartService.SaveCart(cart);
//            }
//            return RedirectToAction("CartIndex");
//        }


//        [HttpPost]
//        public async Task<IActionResult> Checkout(Dictionary<Guid, ProductDto> products)
//        {
//            var userIdString = HttpContext.Session.GetString("UserId");
//            if (userIdString == null)
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Redirect if not logged in
//            }

//            if (!Guid.TryParse(userIdString, out Guid userId))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Handle error if user ID can't be parsed
//            }

//            // Prepare the order
//            var order = new Order
//            {
//                OrderId = Guid.NewGuid(),
//                UserId = userId,
//                CreatedAt = DateTime.UtcNow,
//                Status = "Pending",
//                OrderItems = new List<OrderItem>()
//            };

//            decimal totalAmount = 0;

//            foreach (var product in products.Values)
//            {
//                var orderItem = new OrderItem
//                {
//                    ProductId = product.ProductId,
//                    Quantity = product.Quantity,
//                    Price = product.Price
//                };

//                order.OrderItems.Add(orderItem);
//                totalAmount += product.Price * product.Quantity;

//                // Update product stock
              

//            }
          
//            order.TotalAmount = totalAmount;

//            // Add the order to the database
//            _context.orders.Add(order);
//            await _context.SaveChangesAsync();

//            // Redirect to Bills view with the created OrderId
//            return RedirectToAction("Bills", "Home", new { orderId = order.OrderId });
//        }

    





//        // Hiển thị hóa đơn
//        public async Task<IActionResult> Bills(Guid orderId)
//        {
//            var username = HttpContext.Session.GetString("Username");

//            if (string.IsNullOrEmpty(username))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
//            }

//            // Lấy thông tin đơn hàng từ cơ sở dữ liệu
//            var order = await _context.orders
//                .Include(o => o.OrderItems)
//                .ThenInclude(oi => oi.Product)
//                .Where(o => o.OrderId == orderId && o.User.Username == username)
//                .Select(o => new
//                {
                    
//                    o.OrderId,
//                    o.CreatedAt,
//                    o.TotalAmount,
//                    UserName = o.User.Username,
//                    OrderItems = o.OrderItems.Select(oi => new
//                    {
//                        oi.Product.img,
//                        oi.Product.Name,
//                        oi.Quantity,
//                        oi.Price
//                    }).ToList()
//                })
//                .FirstOrDefaultAsync();

//            if (order == null)
//            {
//                return NotFound();
//            }

//            // Truyền dữ liệu trực tiếp đến view
//            return View(order);
//        }

//        public async Task<IActionResult> OrdersList()
//        {
            
           
//            // Lấy ID người dùng từ session
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest";
//            if (string.IsNullOrEmpty(username))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
//            }

//            // Lấy danh sách đơn hàng của người dùng từ cơ sở dữ liệu
//            var orders = await _context.orders
//                .Where(o => o.User.Username == username)
//                .OrderByDescending(o => o.CreatedAt)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                    .Where(o => o.Status != "Đã hủy bởi khách hàng" && o.Status != "Pending")// Bao gồm thông tin sản phẩm cho mỗi đơn hàng
//                .ToListAsync();

//            var orderDetails = orders.Select(order => new OrderDetailsDto
//            {
//                OrderId = order.OrderId,
//                CreatedAt = order.CreatedAt,
//                TotalAmount = order.TotalAmount,
//                Status=order.Status,
//                Products = order.OrderItems.Select(oi => new ProductDto
//                {
                    
//                    ProductId = oi.ProductId,
//                    Name = oi.Product.Name,
//                    Price = oi.Price,
//                    img = oi.Product.img,
                
//                    Quantity = oi.Quantity
//                }).ToList()
//            }).ToList();

//            return View(orderDetails);
//        }

//        public async Task<IActionResult> OrdersSuccessUser()
//        {


//            // Lấy ID người dùng từ session
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest";
//            if (string.IsNullOrEmpty(username))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
//            }

//            // Lấy danh sách đơn hàng của người dùng từ cơ sở dữ liệu
//            var orders = await _context.orders
//                .Where(o => o.User.Username == username)
//                .OrderByDescending(o => o.CreatedAt)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                     .Where(o => o.Status == "Đã xong" )// Bao gồm thông tin sản phẩm cho mỗi đơn hàng
//                .ToListAsync();

//            var orderDetails = orders.Select(order => new OrderDetailsDto
//            {
//                OrderId = order.OrderId,
//                CreatedAt = order.CreatedAt,
//                TotalAmount = order.TotalAmount,
//                Status = order.Status,
//                Products = order.OrderItems.Select(oi => new ProductDto
//                {

//                    ProductId = oi.ProductId,
//                    Name = oi.Product.Name,
//                    Price = oi.Price,
//                    img = oi.Product.img,

//                    Quantity = oi.Quantity
//                }).ToList()
//            }).ToList();

//            return View(orderDetails);
//        }
//        public async Task<IActionResult> OrdersDeleteUser()
//        {


//            // Lấy ID người dùng từ session
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest";
//            if (string.IsNullOrEmpty(username))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
//            }

//            // Lấy danh sách đơn hàng của người dùng từ cơ sở dữ liệu
//            var orders = await _context.orders
//                .Where(o => o.User.Username == username)
//                .OrderByDescending(o => o.CreatedAt)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                     .Where(o => o.Status == "Đã xóa" )// Bao gồm thông tin sản phẩm cho mỗi đơn hàng
//                .ToListAsync();

//            var orderDetails = orders.Select(order => new OrderDetailsDto
//            {
//                OrderId = order.OrderId,
//                CreatedAt = order.CreatedAt,
//                TotalAmount = order.TotalAmount,
//                Status = order.Status,
//                Products = order.OrderItems.Select(oi => new ProductDto
//                {

//                    ProductId = oi.ProductId,
//                    Name = oi.Product.Name,
//                    Price = oi.Price,
//                    img = oi.Product.img,

//                    Quantity = oi.Quantity
//                }).ToList()
//            }).ToList();

//            return View(orderDetails);
//        }

//        public async Task<IActionResult> OrdersDeleteByUser()
//        {


//            // Lấy ID người dùng từ session
//            var username = HttpContext.Session.GetString("Username");
//            ViewData["Username"] = username ?? "Guest";
//            if (string.IsNullOrEmpty(username))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
//            }

//            // Lấy danh sách đơn hàng của người dùng từ cơ sở dữ liệu
//            var orders = await _context.orders
//                .Where(o => o.User.Username == username)
//                .OrderByDescending(o => o.CreatedAt)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                     .Where(o => o.Status == "Đã hủy bởi khách hàng")// Bao gồm thông tin sản phẩm cho mỗi đơn hàng
//                .ToListAsync();

//            var orderDetails = orders.Select(order => new OrderDetailsDto
//            {
//                OrderId = order.OrderId,
//                CreatedAt = order.CreatedAt,
//                TotalAmount = order.TotalAmount,
//                Status = order.Status,
                
//                Products = order.OrderItems.Select(oi => new ProductDto
//                {

//                    ProductId = oi.ProductId,
//                    Name = oi.Product.Name,
//                    Price = oi.Price,
//                    img = oi.Product.img,

//                    Quantity = oi.Quantity
//                }).ToList()

//            }).ToList();

//            return View(orderDetails);
//        }

//        [HttpPost]
//        public async Task<IActionResult> ConfirmPurchase(Guid orderId, string paymentMethod)
//        {
//            var userIdString = HttpContext.Session.GetString("UserId");
//            if (userIdString == null)
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Chuyển hướng nếu chưa đăng nhập
//            }

//            if (!Guid.TryParse(userIdString, out Guid userId))
//            {
//                return RedirectToAction("IndexLogin", "Login"); // Xử lý lỗi nếu không thể phân tích ID người dùng
//            }

//            var order = await _context.orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
//            if (order == null)
//            {
//                return NotFound(); // Xử lý khi không tìm thấy đơn hàng
//            }

//            order.Status = paymentMethod == "Prepaid" ? "Prepaid" : "Delivery";

//            // Cập nhật số lượng Stock của sản phẩm
//            foreach (var item in order.OrderItems)
//            {
//                var product = await _context.products.FindAsync(item.ProductId);
//                if (product != null)
//                {
//                    product.Stock -= item.Quantity;
//                    if (product.Stock < 0)
//                    {
//                        // Xử lý khi số lượng stock âm (nếu cần thiết)
//                        product.Stock = 0;
//                    }
//                }
//            }

//            await _context.SaveChangesAsync();

//            return RedirectToAction("OrdersList", new { orderId = order.OrderId });
//        }
//        [HttpPost]
//        public async Task<IActionResult> ConfirmDelete(Guid orderId)
//        {
//            var order = await _context.orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.OrderId == orderId);
//            if (order == null)
//            {
//                return NotFound();
//            }

//            // Cập nhật trạng thái của đơn hàng
//            order.Status = "Đã hủy bởi khách hàng";

//            foreach (var item in order.OrderItems)
//            {
//                var product = item.Product; // Sử dụng liên kết đã bao gồm sản phẩm

//                if (product != null)
//                {
//                    product.Stock += item.Quantity; // Cộng số lượng vào kho
//                }
//            }

//            await _context.SaveChangesAsync();

//            return RedirectToAction("OrdersList");
//        }
//        [HttpPost]
//        public async Task<IActionResult> Confirm(Guid orderId)
//        {
//            var order = await _context.orders.FindAsync(orderId);
//            if (order == null)
//            {
//                return NotFound();
//            }

//            order.Status = "Đã nhận được hàng";

//            await _context.SaveChangesAsync();

//            return RedirectToAction("OrdersList");
//        }

//        [HttpGet]
//        public async Task<IActionResult> ExportInvoiceToExcel(Guid orderId)
//        {
//            var order = await _context.orders
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .Include(o => o.User) // Bao gồm User để lấy Username
//                .FirstOrDefaultAsync(o => o.OrderId == orderId);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            var stream = new MemoryStream();

//            using (var package = new ExcelPackage(stream))
//            {
//                var worksheet = package.Workbook.Worksheets.Add("Invoice");

//                // Header
//                worksheet.Cells["A1"].Value = "Invoice";
//                worksheet.Cells["A1:D1"].Merge = true;
//                worksheet.Cells["A1"].Style.Font.Bold = true;
//                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

//                worksheet.Cells["A2"].Value = "Order ID";
//                worksheet.Cells["B2"].Value = order.OrderId.ToString();

//                worksheet.Cells["A3"].Value = "Customer";
//                worksheet.Cells["B3"].Value = order.User.Username; // Chỉnh sửa để lấy Username

//                worksheet.Cells["A4"].Value = "Order Date";
//                worksheet.Cells["B4"].Value = order.CreatedAt.ToString("dd/MM/yyyy");
//                worksheet.Cells["A5"].Value = "Trạng thái";
//                worksheet.Cells["B5"].Value = order.Status;

//                // Table Header
//                worksheet.Cells["A6"].Value = "Product Name";
//                worksheet.Cells["B6"].Value = "Quantity";
//                worksheet.Cells["C6"].Value = "Price";
//                worksheet.Cells["D6"].Value = "Total";
//                worksheet.Cells["A6:D6"].Style.Font.Bold = true;

//                // Order Items
//                var row = 7;
//                foreach (var item in order.OrderItems)
//                {
//                    worksheet.Cells[row, 1].Value = item.Product.Name;
//                    worksheet.Cells[row, 2].Value = item.Quantity;
//                    worksheet.Cells[row, 3].Value = item.Price;
//                    worksheet.Cells[row, 4].Value = item.Price * item.Quantity;
//                    row++;
//                }

//                // Total Amount
         
//                worksheet.Cells[row, 3].Value = "Total";
//                worksheet.Cells[row, 3].Style.Font.Bold = true;
//                worksheet.Cells[row, 4].Value = order.TotalAmount;

//                // Format Price Columns
//                worksheet.Cells[7, 3, row - 1, 3].Style.Numberformat.Format = "$#,##0.00"; // Format for price in USD
//                worksheet.Cells[7, 4, row - 1, 4].Style.Numberformat.Format = "$#,##0.00"; // Format for total per item in USD
//                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00"; // Format for total amount in USD
//                package.Save();
//            }

//            stream.Position = 0;
//            var fileName = $"Invoice_{order.OrderId}.xlsx";
//            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
//        }

//    }
//}


