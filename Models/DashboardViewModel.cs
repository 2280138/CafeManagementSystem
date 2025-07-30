namespace CafeManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }          // ✅ Add this
        public decimal TotalRevenue { get; set; }
        public int TotalReservations { get; set; }
        public int TodayReservations { get; set; }      // ✅ Add this
        public int LowStockItems { get; set; }
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }       // ✅ Add this
    }
}
