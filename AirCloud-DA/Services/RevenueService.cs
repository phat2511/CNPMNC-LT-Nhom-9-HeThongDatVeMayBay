using AirCloud_DA.Data;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly AirCloudDbContext _context;

        public RevenueService(AirCloudDbContext context)
        {
            _context = context;
        }

        public async Task<List<RevenueReport>> GetRevenueReportAsync(DateTime fromDate, DateTime toDate)
        {
            var bookings = await _context.Bookings
                .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate)
                .GroupBy(b => b.CreatedAt.Value.Date)
                .Select(g => new RevenueReport
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(b => b.TotalAmount),
                    TotalBookings = g.Count(),
                    AverageOrderValue = g.Average(b => b.TotalAmount)
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            return bookings;
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Bookings
                .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate)
                .SumAsync(b => b.TotalAmount);
        }

        public async Task<int> GetTotalBookingsAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Bookings
                .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate)
                .CountAsync();
        }

        public async Task<List<Booking>> GetBookingsAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Bookings
                .Include(b => b.Account)
                .Include(b => b.Payments)
                .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<RevenueSummary> GetRevenueSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            var bookings = await _context.Bookings
                .Where(b => b.CreatedAt >= fromDate && b.CreatedAt <= toDate)
                .ToListAsync();

            return new RevenueSummary
            {
                TotalRevenue = bookings.Sum(b => b.TotalAmount),
                TotalBookings = bookings.Count,
                AverageOrderValue = bookings.Any() ? bookings.Average(b => b.TotalAmount) : 0,
                ConfirmedBookings = bookings.Count(b => b.BookingStatus == "Confirmed"),
                PendingBookings = bookings.Count(b => b.BookingStatus == "Pending"),
                CancelledBookings = bookings.Count(b => b.BookingStatus == "Cancelled")
            };
        }
    }
}