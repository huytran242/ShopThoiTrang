using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThoiTrang.Models;
using Microsoft.AspNetCore.Http;
namespace WebThoiTrang.Controllers
{
    public class LoginController : Controller
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
            if (ModelState.IsValid)
            {
                var user = await _context.users.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);
                var admin = await _context.admins.FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);
                if (user != null)
                {
                    // Authentication logic here
                    // For example, you could use ASP.NET Core Identity or set a cookie

                    // Placeholder for setting up authentication
                    // await SignInAsync(user);
                    HttpContext.Session.SetString("Username", user.Username);
                    return RedirectToAction("IndexShop", "Home");
                }

                if (admin != null)
                {
                    // Authentication logic here
                    // For example, you could use ASP.NET Core Identity or set a cookie

                    // Placeholder for setting up authentication
                    // await SignInAsync(user);
                    HttpContext.Session.SetString("Username", admin.Username);
                    return RedirectToAction("IndexAdmin", "Admin");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);

        }
    }
}
