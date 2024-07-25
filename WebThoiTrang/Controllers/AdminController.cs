using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using WebThoiTrang.Models;

namespace WebThoiTrang.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly DbContextShop _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(ILogger<AdminController> logger, DbContextShop context, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostEnvironment = hostEnvironment;
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
                return View("Error"); // Trả về view lỗi nếu dữ liệu bị null
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

        public async Task<IActionResult> ProductAdmin()
        {
            var products = await _context.products.Include(p => p.Category).ToListAsync();
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
            var completedOrders = await _context.orders
                .Where(o => o.Status == "Pending")
                .ToListAsync();

            if (completedOrders == null || !completedOrders.Any())
            {
                return null; // Trả về null nếu không có đơn hàng hoàn thành
            }

            var dailyRevenue = completedOrders
                .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date)
                .Sum(o => o.TotalAmount);

            var weeklyRevenue = completedOrders
                .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .Sum(o => o.TotalAmount);

            var monthlyRevenue = completedOrders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenue
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => o.TotalAmount)
                }).ToList();

            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);

            return new RevenueSummaryViewModel
            {
                DailyRevenue = dailyRevenue,
                WeeklyRevenue = weeklyRevenue,
                MonthlyRevenue = monthlyRevenue,
                TotalRevenue = totalRevenue
            };
        }
      


    }
}

