
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Models;

namespace CafeManagement.Controllers
{
    public class KotsController : Controller
    {
        private readonly CafeDbContext _context;

        public KotsController(CafeDbContext context)
        {
            _context = context;
        }

        private bool HasAccess(params string[] roles)
        {
            var userRole = HttpContext.Session.GetString("Role");
            return userRole != null && roles.Contains(userRole);
        }

        // GET: Kots
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            var cafeDbContext = _context.Kots.Include(k => k.Order);
            return View(await cafeDbContext.ToListAsync());
        }

        // GET: Kots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var kot = await _context.Kots
                .Include(k => k.Order)
                .FirstOrDefaultAsync(m => m.Kotid == id);
            if (kot == null) return NotFound();

            return View(kot);
        }

        // GET: Kots/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            return View();
        }

        // POST: Kots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Kotid,OrderId,Status,GeneratedTime")] Kot kot)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(kot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", kot.OrderId);
            return View(kot);
        }

        // GET: Kots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var kot = await _context.Kots.FindAsync(id);
            if (kot == null) return NotFound();

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", kot.OrderId);
            return View(kot);
        }

        // POST: Kots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Kotid,OrderId,Status,GeneratedTime")] Kot kot)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != kot.Kotid) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KotExists(kot.Kotid)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", kot.OrderId);
            return View(kot);
        }

        // GET: Kots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var kot = await _context.Kots
                .Include(k => k.Order)
                .FirstOrDefaultAsync(m => m.Kotid == id);
            if (kot == null) return NotFound();

            return View(kot);
        }

        // POST: Kots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var kot = await _context.Kots.FindAsync(id);
            if (kot != null)
            {
                _context.Kots.Remove(kot);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool KotExists(int id)
        {
            return _context.Kots.Any(e => e.Kotid == id);
        }
    }
}