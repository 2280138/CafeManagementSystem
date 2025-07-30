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
    public class EmployeeShiftsController : Controller
    {
        private readonly CafeDbContext _context;

        public EmployeeShiftsController(CafeDbContext context)
        {
            _context = context;
        }

        private bool HasAccess(params string[] roles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && roles.Contains(role);
        }

        // GET: EmployeeShifts
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            var cafeDbContext = _context.EmployeeShifts.Include(e => e.User);
            return View(await cafeDbContext.ToListAsync());
        }

        // GET: EmployeeShifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var employeeShift = await _context.EmployeeShifts
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ShiftId == id);

            if (employeeShift == null)
                return NotFound();

            return View(employeeShift);
        }

        // GET: EmployeeShifts/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: EmployeeShifts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShiftId,UserId,ShiftDate,ShiftTime,Task")] EmployeeShift employeeShift)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(employeeShift);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", employeeShift.UserId);
            return View(employeeShift);
        }

        // GET: EmployeeShifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var employeeShift = await _context.EmployeeShifts.FindAsync(id);
            if (employeeShift == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", employeeShift.UserId);
            return View(employeeShift);
        }

        // POST: EmployeeShifts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShiftId,UserId,ShiftDate,ShiftTime,Task")] EmployeeShift employeeShift)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != employeeShift.ShiftId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeShift);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeShiftExists(employeeShift.ShiftId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", employeeShift.UserId);
            return View(employeeShift);
        }

        // GET: EmployeeShifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var employeeShift = await _context.EmployeeShifts
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ShiftId == id);
            if (employeeShift == null)
                return NotFound();

            return View(employeeShift);
        }

        // POST: EmployeeShifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var employeeShift = await _context.EmployeeShifts.FindAsync(id);
            if (employeeShift != null)
            {
                _context.EmployeeShifts.Remove(employeeShift);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeShiftExists(int id)
        {
            return _context.EmployeeShifts.Any(e => e.ShiftId == id);
        }
    }
}