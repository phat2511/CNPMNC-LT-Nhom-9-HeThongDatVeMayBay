using AirCloud_DA.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace AirCloud_DA.Services
{
    public class SupportService : ISupportService
    {
        private readonly AirCloudDbContext _context;

        public SupportService(AirCloudDbContext context)
        {
            _context = context;
        }

        // Dùng cho trang Admin: Lấy tất cả đánh giá đã duyệt
        public async Task<List<SupportTicket>> GetApprovedReviewsAsync()
        {
            try
            {
                // Lọc theo Status == "Resolved" (đã được duyệt)
                return await _context.SupportTickets
                    .Where(t => t.Status == "Resolved" && t.Subject != null && t.Subject.Contains("Đánh giá"))
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return new List<SupportTicket>();
            }
        }

        // Dùng cho trang Khách hàng: Lấy tất cả đánh giá đã duyệt
        public async Task<List<SupportTicket>> GetAllReviewsAsync()
        {
            try
            {
                // Lọc theo Status == "Resolved"
                return await _context.SupportTickets
                    .Where(t =>
                        t.Status == "Resolved"
                        && t.Subject != null
                        && t.Subject.Contains("Đánh giá"))
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return new List<SupportTicket>();
            }
        }

        public async Task<SupportTicket?> GetReviewByIdAsync(int id)
        {
            return await _context.SupportTickets
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        // Tạo đánh giá mới (Sử dụng Status đã được đặt trong CreateModel)
        public async Task CreateReviewAsync(SupportTicket review)
        {
            review.CreatedAt = DateTime.Now;
            // KHÔNG GHI ĐÈ Status hoặc Subject ở đây.
            _context.SupportTickets.Add(review);
            await _context.SaveChangesAsync();
        }

        // Duyệt đánh giá (Đặt Status = "Resolved")
        public async Task ApproveReviewAsync(int id)
        {
            var review = await _context.SupportTickets.FindAsync(id);
            if (review != null)
            {
                review.Status = "Resolved";
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.SupportTickets.FindAsync(id);
            if (review != null)
            {
                _context.SupportTickets.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        // Lấy tất cả đánh giá cho admin (bao gồm cả chờ duyệt và đã duyệt)
        public async Task<List<SupportTicket>> GetAllReviewsForAdminAsync()
        {
            try
            {
                return await _context.SupportTickets
                    .Where(t => t.Subject != null && t.Subject.Contains("Đánh giá"))
                    .Include(t => t.Account)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return new List<SupportTicket>();
            }
        }

        // Từ chối đánh giá (Đặt Status = "Rejected")
        public async Task RejectReviewAsync(int id)
        {
            var review = await _context.SupportTickets.FindAsync(id);
            if (review != null)
            {
                review.Status = "Rejected";
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm và lọc đánh giá
        public async Task<List<SupportTicket>> SearchReviewsAsync(string? searchTerm, string? status)
        {
            try
            {
                var query = _context.SupportTickets
                    .Where(t => t.Subject != null && t.Subject.Contains("Đánh giá"))
                    .Include(t => t.Account)
                    .AsQueryable();

                // Filter theo status
                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query = query.Where(t => t.Status == status);
                }

                // Search theo tiêu đề hoặc nội dung
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(t =>
                        (t.Subject != null && t.Subject.ToLower().Contains(searchTerm)) ||
                        (t.Content != null && t.Content.ToLower().Contains(searchTerm))
                    );
                }

                return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            }
            catch
            {
                return new List<SupportTicket>();
            }
        }

        // Thống kê đánh giá
        public async Task<Dictionary<string, int>> GetReviewStatisticsAsync()
        {
            try
            {
                var reviews = await _context.SupportTickets
                    .Where(t => t.Subject != null && t.Subject.Contains("Đánh giá"))
                    .ToListAsync();

                return new Dictionary<string, int>
                {
                    { "Total", reviews.Count },
                    { "Pending", reviews.Count(r => r.Status == "Open" || r.Status == null) },
                    { "Approved", reviews.Count(r => r.Status == "Resolved") },
                    { "Rejected", reviews.Count(r => r.Status == "Rejected") }
                };
            }
            catch
            {
                return new Dictionary<string, int>
                {
                    { "Total", 0 },
                    { "Pending", 0 },
                    { "Approved", 0 },
                    { "Rejected", 0 }
                };
            }
        }
    }
}