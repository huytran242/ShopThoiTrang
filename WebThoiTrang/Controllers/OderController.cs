//using Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using WebThoiTrang.Models;

//namespace WebThoiTrang.Controllers
//{
//    public class OrderController : Controller
//    {
//        private readonly DbContextShop _context;

//        public OrderController(DbContextShop context)
//        {
//            _context = context;
//        }

//        // Action để hiển thị danh sách đơn hàng
//        public async Task<IActionResult> Index()
//        {
//            var orders = await _context.orders
//                .Include(o => o.User)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .ToListAsync();
//            return View(orders);
//        }

//        // Action để hiển thị chi tiết đơn hàng
//        public async Task<IActionResult> Details(Guid id)
//        {
//            var order = await _context.orders
//                .Include(o => o.User)
//                .Include(o => o.OrderItems)
//                    .ThenInclude(oi => oi.Product)
//                .FirstOrDefaultAsync(o => o.OrderId == id);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            return View(order);
//        }

//        // Action để tạo đơn hàng mới (GET)
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // Action để tạo đơn hàng mới (POST)
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Order order)
//        {
//            if (ModelState.IsValid)
//            {
//                order.OrderId = Guid.NewGuid();
//                _context.Add(order);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(order);
//        }

//        // Action để sửa đơn hàng (GET)
//        public async Task<IActionResult> Edit(Guid id)
//        {
//            var order = await _context.orders.FindAsync(id);
//            if (order == null)
//            {
//                return NotFound();
//            }
//            return View(order);
//        }

//        // Action để sửa đơn hàng (POST)
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(Guid id, Order order)
//        {
//            if (id != order.OrderId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(order);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!OrderExists(order.OrderId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(order);
//        }

//        // Action để xóa đơn hàng (GET)
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            var order = await _context.orders
//                .Include(o => o.User)
//                .FirstOrDefaultAsync(o => o.OrderId == id);
//            if (order == null)
//            {
//                return NotFound();
//            }

//            return View(order);
//        }

//        // Action để xóa đơn hàng (POST)
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(Guid id)
//        {
//            var order = await _context.orders.FindAsync(id);
//            _context.orders.Remove(order);
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool OrderExists(Guid id)
//        {
//            return _context.orders.Any(e => e.OrderId == id);
//        }
//    }
//}

