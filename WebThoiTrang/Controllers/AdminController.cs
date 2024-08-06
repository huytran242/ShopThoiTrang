using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using WebThoiTrang.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using WebThoiTrang.Service;
using WebThoiTrang.Interface;


namespace WebThoiTrang.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ILogger<AdminController> _logger;
        private readonly DbContextShop _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(ILogger<AdminController> logger, DbContextShop context, IWebHostEnvironment hostEnvironment, IProductService productService)
        {
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
            _productService = productService;
        }

        public IActionResult Logout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to the login page or home page
            return RedirectToAction("IndexLogin", "Login");
        }

       
        public async Task<IActionResult> IndexAdmin()
        {
            string username = HttpContext.Session.GetString("Username");
            ViewData["Username"] = username;

            var revenueSummary = await GetRevenueSummaryAsync(); // Hàm để lấy dữ liệu doanh thu

            if (revenueSummary == null)
            {
                return View(); 
            }

            // Lấy danh sách sản phẩm bán chạy nhất
            var topSellingProducts = await _context.products
                .Include(p => p.Category) // Bao gồm thông tin danh mục
                .OrderByDescending(p => p.OrderItems.Sum(o => o.Quantity)) // Sắp xếp theo số lượng bán được
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    img = p.img,
                    CategoryName = p.Category.Name,
                    Quantity = p.OrderItems.Sum(o => o.Quantity),  // Tính số lượng bán được
                    TotalRevenue = p.OrderItems.Sum(o => o.Quantity * o.Price)  // Tính số tiền đã bán được
                })
                .Take(10) // Lấy 10 sản phẩm bán chạy nhất
                .ToListAsync();

            // Tạo ViewModel
            var viewModel = new AdminDashboardViewModel
            {
                RevenueSummary = revenueSummary,
                TopSellingProducts = topSellingProducts
            };

            return View(viewModel);
        }
        public async Task<IActionResult> OrdersDelete(DateTime? searchDate)
        {

            var query = _context.orders
                 .Include(o => o.User)
                 .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                 .Where(o => o.Status == "Đã xóa");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            // Return the view with the list of orders
            return View(orderList);
        }
        public async Task<IActionResult> OrdersDeleteByUser(DateTime? searchDate)
        {

            var query = _context.orders
                 .Include(o => o.User)
                 .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                 .Where(o => o.Status == "Đã hủy bởi khách hàng");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            // Return the view with the list of orders
            return View(orderList);
        }
        public async Task<IActionResult> OrdersComFirm(DateTime? searchDate)
        {

            var query = _context.orders
                 .Include(o => o.User)
                 .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                 .Where(o => o.Status == "Đã xác nhận bởi cửa hàng" || o.Status == "Đã xác nhận thanh toán khi nhận hàng");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            // Return the view with the list of orders
            return View(orderList);
        }
        public async Task<IActionResult> DagiaoOrder(DateTime? searchDate)
        {
            // Define the query to include related entities and filter by status
            var query = _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == "Đã giao");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            // Return the view with the list of orders
            return View(orderList);
        }
        public async Task<IActionResult> DanggiaoOrder(DateTime? searchDate)
        {
            // Define the query to include related entities and filter by status
            var query = _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == "Đang giao");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            // Return the view with the list of orders
            return View(orderList);
        }
        public async Task<IActionResult> OrdersSuccsec(DateTime? searchDate)
        {
            // Define the query to include related entities and filter by status
            var query = _context.orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == "Đã nhận được hàng");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }
         
            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            // Return the view with the list of orders
            return View(orderList);
        }

        public async Task<IActionResult> IndexOderzzzzzzz(DateTime? searchDate)
        {


            var query = _context.orders
               .Include(o => o.User)
               .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Product)
           .Where(o => o.Status == "Prepaid" || o.Status == "Delivery");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            if (orderList == null || !orderList.Any())
            {
                // Trả về view với danh sách rỗng
                return View(new List<OrdersListDto>());
            }

            return View(orderList);
        }
        public async Task<IActionResult> OrdersThanhtoan(DateTime? searchDate)
        {


            var query = _context.orders
               .Include(o => o.User)
               .Include(o => o.OrderItems)
                   .ThenInclude(oi => oi.Product)
           .Where(o => o.Status == "Chưa thanh toán");

            // Filter by the searchDate if it is provided
            if (searchDate.HasValue)
            {
                // Assuming the searchDate is for a specific day (e.g., searching for orders on a particular date)
                var startDate = searchDate.Value.Date;
                var endDate = startDate.AddDays(1); // Include the whole day

                query = query.Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate);
            }

            // Retrieve the filtered orders
            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
            // Map the orders to the OrderListDto
            var orderList = orders.Select(order => new OrdersListDto
            {
                OrderId = order.OrderId,
                Username = order.User.Username,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Products = order.OrderItems.Select(oi => new ProductDto
                {
                    ProductId = oi.ProductId,
                    Name = oi.Product.Name,
                    Price = oi.Price,
                    img = oi.Product.img,
                    Quantity = oi.Quantity
                }).ToList()
            }).ToList();

            if (orderList == null || !orderList.Any())
            {
                // Trả về view với danh sách rỗng
                return View(new List<OrdersListDto>());
            }

            return View(orderList);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(Guid orderId,string paymentMethod)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = paymentMethod == "Đã xác nhận bởi cửa hàng" ? "Đã xác nhận bởi cửa hàng" : "Đã xác nhận thanh toán khi nhận hàng";
         
            await _context.SaveChangesAsync();
          
            return RedirectToAction("OrdersComFirm");

        }
        public async Task<IActionResult> ConfirmOrderThanhToan(Guid orderId)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Đã thanh toán";

            await _context.SaveChangesAsync();

            return RedirectToAction("OrdersComFirm");

        }
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Đã xóa";
            foreach (var item in order.OrderItems)
            {
                var product = item.Product; // Sử dụng liên kết đã bao gồm sản phẩm

                if (product != null)
                {
                    product.Stock += item.Quantity; // Cộng số lượng vào kho
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("OrdersDelete");
        }
        
        [HttpPost]
        public async Task<IActionResult> DanggiaoOrder(Guid orderId)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Đang giao";
            await _context.SaveChangesAsync();

            return RedirectToAction("DanggiaoOrder");
        }

        [HttpPost]
        public async Task<IActionResult> DagiaoOrder(Guid orderId)
        {
            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Đã giao ";
            await _context.SaveChangesAsync();

            return RedirectToAction("DagiaoOrder");
        }




        public async Task<IActionResult> ProductAdmin(string searchTerm)
        {
            var products = string.IsNullOrWhiteSpace(searchTerm) ?
                await _context.products.Include(p => p.Category).ToListAsync() :
                await _context.products.Include(p => p.Category)
                                       .Where(p => p.Name.Contains(searchTerm) ||
                                                   p.Description.Contains(searchTerm) ||
                                                   p.Category.Name.Contains(searchTerm))
                                       .ToListAsync();

            ViewData["SearchTerm"] = searchTerm;
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> ProductDetails(Guid id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult CreateProduct()
        {
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name");
            return View();
        } 

        // POST: Products/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile imgFile)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    // Lưu ảnh vào thư mục và lấy đường dẫn
                    string imgPath = await SaveImageAsync(imgFile);

                    // Lưu đường dẫn ảnh vào thuộc tính Img của product
                    product.img = imgPath;


                    // Kiểm tra nếu category tồn tại
                    var category = await _context.categories.FindAsync(product.CategoryId);
                    if (category == null)
                    {
                        ModelState.AddModelError("CategoryId", "Danh mục không hợp lệ.");
                        PopulateCategoriesDropdownList(product.CategoryId); // Trả về dropdownlist với category đã chọn
                        return View(product);
                    }
                    product.Category = category;

                    // Thiết lập các giá trị khác cho product
                    product.ProductId = Guid.NewGuid();
                    product.CreatedAt = DateTime.UtcNow;
                    product.UpdatedAt = DateTime.UtcNow;

                    // Thêm product vào DbContext và lưu thay đổi
                    _context.Add(product);
                    // Lưu sản phẩm vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ProductAdmin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                }
            }
            // Tìm category từ database dựa trên CategoryId của product
                PopulateCategoriesDropdownList(product.CategoryId);
                return View(product);
        }
        private void PopulateCategoriesDropdownList(object selectedCategory = null)
        {
            var categoriesQuery = _context.categories.OrderBy(c => c.Name);
            ViewBag.CategoryId = new SelectList(categoriesQuery.AsNoTracking(), "CategoryId", "Name", selectedCategory);
        }



      
 // GET: Product/Edit/{id}
        public async Task<IActionResult> EditProduct1(Guid id)
        {
            var product = await _context.products
       .Include(p => p.Category) // Nếu cần hiển thị tên danh mục
       .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Load lại danh sách Category (nếu cần thiết)
            ViewBag.CategoryId = new SelectList(_context.categories, "CategoryId", "Name", product.CategoryId);

            return View(product);
        }

        // POST: Product/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
  
        public async Task<IActionResult> EditProduct1(Guid id, [Bind("ProductId,Name,Description,Price,Stock,CategoryId,Img,CreatedAt,UpdatedAt")] Product product, IFormFile imgFile)
        {
            
                if (!ModelState.IsValid)
                {
                    try
                    {
                    var existingProduct = await _context.products.FindAsync(id);
                   
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;
                    existingProduct.CategoryId = product.CategoryId;
                    if (imgFile != null && imgFile.Length > 0)
                    {
                        string imgPath = await SaveImageAsync(imgFile);
                        product.img = imgPath;
                    }
                    else
                    {
                      
                      
                        if (existingProduct != null)
                        {
                            product.img = existingProduct.img;
                        }
                    }

                        // Kiểm tra nếu category tồn tại
                        var category = await _context.categories.FindAsync(product.CategoryId);
                        if (category == null)
                        {
                            ModelState.AddModelError("CategoryId", "Danh mục không hợp lệ.");
                            PopulateCategoriesDropdownList(product.CategoryId); // Trả về dropdownlist với category đã chọn
                            return View(product);
                        }
                       existingProduct.Category = category;
                       existingProduct.UpdatedAt = DateTime.Now;
                 
                        // Lưu sản phẩm vào cơ sở dữ liệu
                        await _context.SaveChangesAsync();

                    
                    }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ProductAdmin");
            }
                // Tìm category từ database dựa trên CategoryId của product
                PopulateCategoriesDropdownList(product.CategoryId);
                return View(product);
            }
        


        private async Task<string> SaveImageAsync(IFormFile imgFile)
        {
            if (imgFile == null || imgFile.Length == 0)
                throw new Exception("Image file is required.");

            // Đường dẫn thư mục lưu trữ ảnh, ví dụ: wwwroot/images
            var uploads = Path.Combine(_hostEnvironment.WebRootPath, "images");

            // Tạo tên file duy nhất để tránh trùng lặp
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imgFile.FileName);
            var filePath = Path.Combine(uploads, uniqueFileName);

            // Lưu file vào thư mục
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imgFile.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn của file lưu trữ
            return "/images/" + uniqueFileName;
        }
        // GET: Products/Delete/5
        public async Task<IActionResult> ProductDelete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("ProductDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.products.FindAsync(id);
            _context.products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProductAdmin");
        }

        private bool ProductExists(Guid id)
        {
            return _context.products.Any(e => e.ProductId == id);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /// <summary>
        /// /////////////////////////////
        /// </summary>
        /// <returns></returns>
        // GET: Categories
            public async Task<IActionResult> IndexCategory()
            {
                return View(await _context.categories.ToListAsync());
            }

            // GET: Categories/Details/5
            public async Task<IActionResult> DetailsCategory(Guid id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var category = await _context.categories
                    .FirstOrDefaultAsync(m => m.CategoryId == id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }

            // GET: Categories/Create
            public IActionResult CreateCategory()
            {
                return View();
            }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("CategoryId,Name,Description")] Category category)
        {
            if (!ModelState.IsValid)
            {
                // Log các lỗi trong ModelState
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"ModelState error: {error.ErrorMessage}");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                category.CategoryId = Guid.NewGuid();
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexCategory");
            }
            return View(category);
        }



        // GET: Categories/Edit/5
        public async Task<IActionResult> EditCategory(Guid id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var category = await _context.categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }

            // POST: Categories/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditCategory(Guid id, [Bind("CategoryId,Name,Description")] Category category)
            {
                if (id != category.CategoryId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.CategoryId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("IndexCategory");
                }
                return View(category);
            }

            // GET: Categories/Delete/5
            public async Task<IActionResult> DeleteCategory(Guid id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var category = await _context.categories
                    .FirstOrDefaultAsync(m => m.CategoryId == id);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }

            // POST: Categories/Delete/5
            [HttpPost, ActionName("DeleteCategory")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteCateConfirmed(Guid id)
            {
                var category = await _context.categories.FindAsync(id);
                _context.categories.Remove(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexCategory");
            }

            private bool CategoryExists(Guid id)
            {
                return _context.categories.Any(e => e.CategoryId == id);
            }

        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <returns></returns>
           
          // GET: Users
        public async Task<IActionResult> UserList()
        {
            return _context.users != null ?
                        View(await _context.users.ToListAsync()) :
                        Problem("Entity set 'DbContextShop.users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> UserDetails(Guid id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var user = await _context.users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create

        private async Task<RevenueSummaryViewModel> GetRevenueSummaryAsync()
        {
            // Fetch orders with status "đã xong"
            var completedOrders = await _context.orders
                .Where(o => o.Status == "đã xong")
                .ToListAsync();

            if (completedOrders == null || !completedOrders.Any())
            {
                return null; // Return null if there are no completed orders
            }

            // Calculate daily revenue
            var dailyRevenue = completedOrders
                .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date)
                .Sum(o => o.TotalAmount);

            // Calculate weekly revenue
            var weeklyRevenue = completedOrders
                .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .Sum(o => o.TotalAmount);

            // Calculate monthly revenue
            var monthlyRevenue = completedOrders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenue
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount)
                }).ToList();

            // Calculate total revenue
            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);

            return new RevenueSummaryViewModel
            {
                DailyRevenue = dailyRevenue,
                WeeklyRevenue = weeklyRevenue,
                MonthlyRevenue = monthlyRevenue,
                TotalRevenue = totalRevenue
            };
        }
        [HttpGet]
        public async Task<IActionResult> ExportInvoiceToExcel(Guid orderId)
        {
            var order = await _context.orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User) // Bao gồm User để lấy Username
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Invoice");

                // Header
                worksheet.Cells["A1"].Value = "Invoice";
                worksheet.Cells["A1:D1"].Merge = true;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A2"].Value = "Order ID";
                worksheet.Cells["B2"].Value = order.OrderId.ToString();

                worksheet.Cells["A3"].Value = "Customer";
                worksheet.Cells["B3"].Value = order.User.Username; // Chỉnh sửa để lấy Username

                worksheet.Cells["A4"].Value = "Order Date";
                worksheet.Cells["B4"].Value = order.CreatedAt.ToString("dd/MM/yyyy");
                worksheet.Cells["A5"].Value = "Trạng thái";
                worksheet.Cells["B5"].Value = order.Status;

                // Table Header
                worksheet.Cells["A6"].Value = "Product Name";
                worksheet.Cells["B6"].Value = "Quantity";
                worksheet.Cells["C6"].Value = "Price";
                worksheet.Cells["D6"].Value = "Total";
                worksheet.Cells["A6:D6"].Style.Font.Bold = true;

                // Order Items
                var row = 7;
                foreach (var item in order.OrderItems)
                {
                    worksheet.Cells[row, 1].Value = item.Product.Name;
                    worksheet.Cells[row, 2].Value = item.Quantity;
                    worksheet.Cells[row, 3].Value = item.Price;
                    worksheet.Cells[row, 4].Value = item.Price * item.Quantity;
                    row++;
                }

                // Total Amount

                worksheet.Cells[row, 3].Value = "Total";
                worksheet.Cells[row, 3].Style.Font.Bold = true;
                worksheet.Cells[row, 4].Value = order.TotalAmount;

                // Format Price Columns
                worksheet.Cells[7, 3, row - 1, 3].Style.Numberformat.Format = "$#,##0.00"; // Format for price in USD
                worksheet.Cells[7, 4, row - 1, 4].Style.Numberformat.Format = "$#,##0.00"; // Format for total per item in USD
                worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00"; // Format for total amount in USD
                package.Save();
            }

            stream.Position = 0;
            var fileName = $"Invoice_{order.OrderId}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }



    }
}

