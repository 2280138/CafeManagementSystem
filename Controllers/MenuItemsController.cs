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
    public class MenuItemsController : Controller
    {
        private readonly CafeDbContext _context;

        public MenuItemsController(CafeDbContext context)
        {
            _context = context;
        }

        // 🔐 Role Checker
        private bool HasAccess(params string[] allowedRoles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && allowedRoles.Contains(role);
        }

        // GET: MenuItems
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            return View(await _context.MenuItems.ToListAsync());
        }

        // GET: MenuItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        // GET: MenuItems/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View();
        }

        // POST: MenuItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuItemId,Name,Category,Price,IsAvailable")] MenuItem menuItem)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(menuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menuItem);
        }

        // GET: MenuItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        // POST: MenuItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuItemId,Name,Category,Price,IsAvailable")] MenuItem menuItem)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != menuItem.MenuItemId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.MenuItemId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menuItem);
        }

        // GET: MenuItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
            if (menuItem == null) return NotFound();

            return View(menuItem);
        }

        // POST: MenuItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.MenuItemId == id);
        }
    }
}