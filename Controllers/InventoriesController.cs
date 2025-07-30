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
    public class InventoriesController : Controller
    {
        private readonly CafeDbContext _context;

        public InventoriesController(CafeDbContext context)
        {
            _context = context;
        }

        private bool HasAccess(params string[] roles)
        {
            var userRole = HttpContext.Session.GetString("Role");
            return userRole != null && roles.Contains(userRole);
        }

        // GET: Inventories
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            return View(await _context.Inventories.ToListAsync());
        }

        // GET: Inventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // GET: Inventories/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View();
        }

        // POST: Inventories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InventoryId,ItemName,QuantityInStock,Unit,MinStockLevel")] Inventory inventory)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: Inventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // POST: Inventories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InventoryId,ItemName,QuantityInStock,Unit,MinStockLevel")] Inventory inventory)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != inventory.InventoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.InventoryId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        // GET: Inventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(m => m.InventoryId == id);
            if (inventory == null)
                return NotFound();

            return View(inventory);
        }

        // POST: Inventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory != null)
            {
                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventories.Any(e => e.InventoryId == id);
        }
    }
}