using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CafeManagement.Models;
using System.Linq;

public class DashboardController : Controller
{
    private readonly CafeDbContext _context;

    public DashboardController(CafeDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Username") == null)
            return RedirectToAction("Login", "Account");

        var today = DateTime.Today;

        var model = new DashboardViewModel
        {
            TotalOrders = _context.Orders.Count(o => o.OrderDate.Date == today),
            PendingOrders = _context.Orders.Count(o => o.OrderDate.Date == today && o.Status == "Pending"),
            TotalRevenue = _context.Billings
                .Include(b => b.Order)
               .Where(b => b.Order != null && b.Order.OrderDate.Date == today)
                .Sum(b => (decimal?)b.AmountPaid) ?? 0,
            TotalReservations = _context.Reservations.Count(),
            TodayReservations = _context.Reservations.Count(r => r.ReservationDate.Date == today),
            LowStockItems = _context.Inventories.Count(i => i.QuantityInStock <= i.MinStockLevel),
            TotalFeedbacks = _context.Feedbacks.Count(),
            AverageRating = _context.Feedbacks.Any()
                            ? _context.Feedbacks.Average(f => f.Rating)
                            : 0
        };

        return View(model);
    }
}
