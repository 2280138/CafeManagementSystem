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
    public class OrderItemsController : Controller
    {
        private readonly CafeDbContext _context;

        public OrderItemsController(CafeDbContext context)
        {
            _context = context;
        }

        // 🔐 Role checker
        private bool HasAccess(params string[] allowedRoles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && allowedRoles.Contains(role);
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var cafeDbContext = _context.OrderItems.Include(o => o.MenuItem).Include(o => o.Order);
            return View(await cafeDbContext.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems
                .Include(o => o.MenuItem)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderItemId == id);
            if (orderItem == null)
                return NotFound();

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewData["MenuItemId"] = new SelectList(_context.MenuItems, "MenuItemId", "MenuItemId");
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            return View();
        }

        // POST: OrderItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,MenuItemId,Quantity")] OrderItem orderItem)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MenuItemId"] = new SelectList(_context.MenuItems, "MenuItemId", "MenuItemId", orderItem.MenuItemId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
                return NotFound();

            ViewData["MenuItemId"] = new SelectList(_context.MenuItems, "MenuItemId", "MenuItemId", orderItem.MenuItemId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderItem.OrderId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderItemId,OrderId,MenuItemId,Quantity")] OrderItem orderItem)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != orderItem.OrderItemId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.OrderItemId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MenuItemId"] = new SelectList(_context.MenuItems, "MenuItemId", "MenuItemId", orderItem.MenuItemId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var orderItem = await _context.OrderItems
                .Include(o => o.MenuItem)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderItemId == id);
            if (orderItem == null)
                return NotFound();

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.OrderItemId == id);
        }
    }
}