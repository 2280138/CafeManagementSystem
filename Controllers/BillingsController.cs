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
    public class BillingsController : Controller
    {
        private readonly CafeDbContext _context;

        public BillingsController(CafeDbContext context)
        {
            _context = context;
        }

        private bool HasAccess(params string[] roles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && roles.Contains(role);
        }

        // GET: Billings
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            var cafeDbContext = _context.Billings.Include(b => b.Order);
            return View(await cafeDbContext.ToListAsync());
        }

        // GET: Billings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var billing = await _context.Billings
                .Include(b => b.Order)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (billing == null)
                return NotFound();

            return View(billing);
        }

        // GET: Billings/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            return View();
        }

        // POST: Billings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillId,OrderId,PaymentMethod,AmountPaid")] Billing billing)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(billing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", billing.OrderId);
            return View(billing);
        }

        // GET: Billings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var billing = await _context.Billings.FindAsync(id);
            if (billing == null)
                return NotFound();

            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", billing.OrderId);
            return View(billing);
        }

        // POST: Billings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillId,OrderId,PaymentMethod,AmountPaid")] Billing billing)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != billing.BillId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillingExists(billing.BillId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", billing.OrderId);
            return View(billing);
        }

        // GET: Billings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var billing = await _context.Billings
                .Include(b => b.Order)
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (billing == null)
                return NotFound();

            return View(billing);
        }

        // POST: Billings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var billing = await _context.Billings.FindAsync(id);
            if (billing != null)
            {
                _context.Billings.Remove(billing);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BillingExists(int id)
        {
            return _context.Billings.Any(e => e.BillId == id);
        }
    }
}
