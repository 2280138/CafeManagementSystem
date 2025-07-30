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
    public class SuppliersController : Controller
    {
        private readonly CafeDbContext _context;

        public SuppliersController(CafeDbContext context)
        {
            _context = context;
        }

        // ✅ Helper method to check if user has allowed role
        private bool HasAccess(params string[] allowedRoles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && allowedRoles.Contains(role);
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View(await _context.Suppliers.ToListAsync());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View();
        }

        // POST: Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,SupplierName,ContactInfo")] Supplier supplier)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,SupplierName,ContactInfo")] Supplier supplier)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != supplier.SupplierId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
                _context.Suppliers.Remove(supplier);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}