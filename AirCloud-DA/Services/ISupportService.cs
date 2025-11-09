using AirCloud_DA.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirCloud_DA.Services
{
    public interface ISupportService
    {
        Task<List<SupportTicket>> GetApprovedReviewsAsync();

        Task<List<SupportTicket>> GetAllReviewsAsync();

        Task<SupportTicket?> GetReviewByIdAsync(int id);
        Task CreateReviewAsync(SupportTicket review);
        Task ApproveReviewAsync(int id);
        Task DeleteReviewAsync(int id);

        // Thêm các method mới cho admin
        Task<List<SupportTicket>> GetAllReviewsForAdminAsync();
        Task RejectReviewAsync(int id);
        Task<List<SupportTicket>> SearchReviewsAsync(string? searchTerm, string? status);
        Task<Dictionary<string, int>> GetReviewStatisticsAsync();
    }
}