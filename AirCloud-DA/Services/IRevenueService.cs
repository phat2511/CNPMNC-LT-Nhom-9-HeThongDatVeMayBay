using AirCloud_DA.Data;

namespace AirCloud_DA.Services
{
    public interface IRevenueService
    {
        Task<List<RevenueReport>> GetRevenueReportAsync(DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);
        Task<int> GetTotalBookingsAsync(DateTime fromDate, DateTime toDate);
        Task<List<Booking>> GetBookingsAsync(DateTime fromDate, DateTime toDate);
        Task<RevenueSummary> GetRevenueSummaryAsync(DateTime fromDate, DateTime toDate);
    }

    public class RevenueReport
    {
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class RevenueSummary
    {
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CancelledBookings { get; set; }
    }
}