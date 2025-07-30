using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Models;

namespace CafeManagement.Controllers
{
    public class WaitTimesController : Controller
    {
        private readonly CafeDbContext _context;

        public WaitTimesController(CafeDbContext context)
        {
            _context = context;
        }

        private bool HasAccess(params string[] allowedRoles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && allowedRoles.Contains(role);
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var cafeDbContext = _context.WaitTimes.Include(w => w.Order);
            return View(await cafeDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var waitTime = await _context.WaitTimes
                .Include(w => w.Order)
                .FirstOrDefaultAsync(m => m.WaitTimeId == id);
            if (waitTime == null)
                return NotFound();

            return View(waitTime);
        }

        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WaitTimeId,OrderId,EstimatedTime")] WaitTime waitTime)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(waitTime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", waitTime.OrderId);
            return View(waitTime);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var waitTime = await _context.WaitTimes.FindAsync(id);
            if (waitTime == null)
                return NotFound();

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", waitTime.OrderId);
            return View(waitTime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WaitTimeId,OrderId,EstimatedTime")] WaitTime waitTime)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != waitTime.WaitTimeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waitTime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaitTimeExists(waitTime.WaitTimeId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", waitTime.OrderId);
            return View(waitTime);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var waitTime = await _context.WaitTimes
                .Include(w => w.Order)
                .FirstOrDefaultAsync(m => m.WaitTimeId == id);
            if (waitTime == null)
                return NotFound();

            return View(waitTime);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var waitTime = await _context.WaitTimes.FindAsync(id);
            if (waitTime != null)
                _context.WaitTimes.Remove(waitTime);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaitTimeExists(int id)
        {
            return _context.WaitTimes.Any(e => e.WaitTimeId == id);
        }
    }
}
