using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeManagement.Models;

namespace CafeManagement.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly CafeDbContext _context;

        public ReservationsController(CafeDbContext context)
        {
            _context = context;
        }

        // 🔒 Role check helper method
        private bool HasAccess(params string[] allowedRoles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && allowedRoles.Contains(role);
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
                return RedirectToAction("Login", "Account");

            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View(await _context.Reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations.FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,CustomerName,ReservationDate,NumberOfPeople,Status")] Reservation reservation)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,CustomerName,ReservationDate,NumberOfPeople,Status")] Reservation reservation)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id != reservation.ReservationId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            if (id == null)
                return NotFound();

            var reservation = await _context.Reservations.FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!HasAccess("Admin", "Manager", "Waiter"))
                return View("~/Views/Shared/AccessDenied.cshtml");

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
