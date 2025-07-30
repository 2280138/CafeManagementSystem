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
    public class LoyaltyProgramsController : Controller
    {
        private readonly CafeDbContext _context;

        public LoyaltyProgramsController(CafeDbContext context)
        {
            _context = context;
        }


        // Role checker helper
        private bool HasAccess(params string[] roles)
        {
            var userRole = HttpContext.Session.GetString("Role");
            return userRole != null && roles.Contains(userRole);
        }

        // GET: LoyaltyPrograms
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            var cafeDbContext = _context.LoyaltyPrograms.Include(l => l.User);
            return View(await cafeDbContext.ToListAsync());
        }

        // GET: LoyaltyPrograms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var loyaltyProgram = await _context.LoyaltyPrograms
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LoyaltyId == id);
            if (loyaltyProgram == null) return NotFound();

            return View(loyaltyProgram);
        }

        // GET: LoyaltyPrograms/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: LoyaltyPrograms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoyaltyId,UserId,Points")] LoyaltyProgram loyaltyProgram)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(loyaltyProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", loyaltyProgram.UserId);
            return View(loyaltyProgram);
        }

        // GET: LoyaltyPrograms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var loyaltyProgram = await _context.LoyaltyPrograms.FindAsync(id);
            if (loyaltyProgram == null) return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", loyaltyProgram.UserId);
            return View(loyaltyProgram);
        }

        // POST: LoyaltyPrograms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoyaltyId,UserId,Points")] LoyaltyProgram loyaltyProgram)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != loyaltyProgram.LoyaltyId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loyaltyProgram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoyaltyProgramExists(loyaltyProgram.LoyaltyId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", loyaltyProgram.UserId);
            return View(loyaltyProgram);
        }

        // GET: LoyaltyPrograms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null) return NotFound();

            var loyaltyProgram = await _context.LoyaltyPrograms
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.LoyaltyId == id);
            if (loyaltyProgram == null) return NotFound();

            return View(loyaltyProgram);
        }

        // POST: LoyaltyPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var loyaltyProgram = await _context.LoyaltyPrograms.FindAsync(id);
            if (loyaltyProgram != null)
            {
                _context.LoyaltyPrograms.Remove(loyaltyProgram);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LoyaltyProgramExists(int id)
        {
            return _context.LoyaltyPrograms.Any(e => e.LoyaltyId == id);
        }
    }
}