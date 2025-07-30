using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Models;
using Microsoft.AspNetCore.Http;

namespace CafeManagement.Controllers
{
    public class UsersController : Controller
    {
        private readonly CafeDbContext _context;

        public UsersController(CafeDbContext context)
        {
            _context = context;
        }

        // 🔐 Reusable role-check method
        private bool HasAccess(string allowedRole)
        {
            var role = HttpContext.Session.GetString("Role");
            return role == allowedRole;
        }

        // ✅ INDEX - Admin Only
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View(await _context.Users.ToListAsync());
        }

        // ✅ DETAILS - Admin Only
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // ✅ CREATE - Admin Only
        public IActionResult Create()
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,Role")] User user)
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // ✅ EDIT - Admin Only
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,Role")] User user)
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != user.UserId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // ✅ DELETE - Admin Only
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var user = await _context.Users.FindAsync(id);
            if (user != null)
                _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
