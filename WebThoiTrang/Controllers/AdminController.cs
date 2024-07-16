﻿using Data;
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
    public class AdminController : Controller
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

        // GET: Products
        public IActionResult IndexAdmin()
        {
            return View();
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
                   
              
                        product.UpdatedAt = DateTime.UtcNow;

                        // Thêm product vào DbContext và lưu thay đổi
                        _context.Update(product);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _context.products.FindAsync(id);
            _context.products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            [HttpPost, ActionName("Delete")]
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
            // GET: Admins
            public async Task<IActionResult> IndexAd()
            {
                return View(await _context.admins.ToListAsync());
            }

            // GET: Admins/Details/5
            public async Task<IActionResult> DetailsAd(Guid id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var admin = await _context.admins
                    .FirstOrDefaultAsync(m => m.AdminId == id);
                if (admin == null)
                {
                    return NotFound();
                }

                return View(admin);
            }

            // GET: Admins/Create
            public IActionResult CreateAd()
            {
                return View();
            }

            // POST: Admins/Create
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CreateAd([Bind("AdminId,Username,Password,Email,CreatedAt")] Admin admin)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(admin);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("IndexCategory");
                }
                return View(admin);
            }

            // GET: Admins/Edit/5
            public async Task<IActionResult> EditAd(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var admin = await _context.admins.FindAsync(id);
                if (admin == null)
                {
                    return NotFound();
                }
                return View(admin);
            }

            // POST: Admins/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditAd(Guid id, [Bind("AdminId,Username,Password,Email,CreatedAt")] Admin admin)
            {
                if (id != admin.AdminId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(admin);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AdminExists(admin.AdminId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(admin);
            }

            // GET: Admins/Delete/5
            public async Task<IActionResult> DeleteAd(Guid id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var admin = await _context.admins
                    .FirstOrDefaultAsync(m => m.AdminId == id);
                if (admin == null)
                {
                    return NotFound();
                }

                return View(admin);
            }

            // POST: Admins/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteAdminConfirmed(Guid id)
            {
                var admin = await _context.admins.FindAsync(id);
                _context.admins.Remove(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool AdminExists(Guid id)
            {
                return _context.admins.Any(e => e.AdminId == id);
            }
        //////////////////////////
        //////////////////////////
    }
}
