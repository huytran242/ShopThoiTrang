using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebThoiTrang.Models;
using Data;

namespace WebThoiTrang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbContextShop _context;
        public HomeController(ILogger<HomeController> logger, DbContextShop context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> IndexShopAsync()
        {
            var products = await _context.products
             .Include(p => p.Category)
             .ToListAsync();

            return View(products);
        }

        public IActionResult Details()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
