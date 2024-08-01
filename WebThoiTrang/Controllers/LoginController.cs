using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Models;
using Microsoft.AspNetCore.Http;
namespace WebThoiTrang.Controllers
{
    public class LoginController : BaseController
    {
     
        private readonly DbContextShop _context;


        public LoginController(DbContextShop context)
        {
            _context = context;
    
        }

        public IActionResult IndexLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IndexLogin(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
<<<<<<< HEAD
                return RedirectToAction("UserCreate", "Home");
            }
           if (ModelState.IsValid)
            {
=======
>>>>>>> fa7bf7715d95be9f883530a630fdf38a38bd80a1
                var user = await _context.users
                    .Where(u => u.Username == model.Username && u.Password == model.Password)
                    .Select(u => new { u.UserId, u.Username })
                    .FirstOrDefaultAsync();

                var admin = await _context.admins
                    .Where(a => a.Username == model.Username && a.Password == model.Password)
                    .Select(a => new { a.AdminId, a.Username })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    // Authentication logic here
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserId", user.UserId.ToString()); // Lưu ID người dùng vào session
                    return RedirectToAction("IndexShop", "Home");
                }

                if (admin != null)
                {
                    // Authentication logic here
                    HttpContext.Session.SetString("Username", admin.Username);
                    HttpContext.Session.SetString("UserId", admin.AdminId.ToString()); // Lưu ID quản trị viên vào session
                    return RedirectToAction("IndexAdmin", "Admin");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }


    }
}

