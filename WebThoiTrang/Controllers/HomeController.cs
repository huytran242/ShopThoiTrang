using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebThoiTrang.Models;
using Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();

            // Redirect to the login page or home page
            return RedirectToAction("IndexShop", "Home");
        }
        public async Task<IActionResult> IndexShop()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewData["Username"] = username ?? "Guest"; // Nếu không có Username trong Session, gán giá trị là "Guest"
            var products = await _context.products
             .Include(p => p.Category)
             .ToListAsync();
            ViewData["Username"] = username;
            return View(products);
        }
        public async Task<IActionResult> Bills(Guid id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var product = await _context.products
            //    .Include(p => p.Category)
            //    .FirstOrDefaultAsync(m => m.ProductId == id);

            //if (product == null)
            //{
            //    return NotFound();
            //}

            return View();
        }
    

        public async Task<IActionResult> Details(Guid id)
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
        public IActionResult UserCreate()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate([Bind("UserId,Username,Password,Email,CreatedAt")] User user)
        {
            if (!ModelState.IsValid)
            {
                var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(user);
                }
                user.Orders = new List<Order>();
                user.UserId = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("IndexShop");
            }
            return View(user);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> UserEdit(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(Guid id, [Bind("UserId,Username,Password,Email,CreatedAt")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("IndexShop");
            }
            return View(user);
        }
        public async Task<IActionResult> UserDelete(Guid id)
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

        // POST: Users/Delete/5
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.users == null)
            {
                return Problem("Entity set 'DbContextShop.users'  is null.");
            }
            var user = await _context.users.FindAsync(id);
            if (user != null)
            {
                _context.users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("IndexShop");
        }

        private bool UserExists(Guid id)
        {
            return (_context.users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
      
    }
}
