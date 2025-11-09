using AirCloud_DA.Services;
using AirCloud_DA.Data;
using SupportTicket = AirCloud_DA.Data.SupportTicket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirCloud_DA.Pages.Admin.Reviews
{
    public class IndexModel : PageModel
    {
        private readonly ISupportService _support;

        public IndexModel(ISupportService support)
        {
            _support = support;
        }

        public List<SupportTicket> Reviews { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string StatusFilter { get; set; } = "All";
        public Dictionary<string, int> Statistics { get; set; } = new();

        public async Task OnGetAsync(string? searchTerm, string? statusFilter)
        {
            SearchTerm = searchTerm;
            StatusFilter = string.IsNullOrEmpty(statusFilter) ? "All" : statusFilter;

            Reviews = await _support.SearchReviewsAsync(SearchTerm, StatusFilter);
            Statistics = await _support.GetReviewStatisticsAsync();
        }



        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _support.DeleteReviewAsync(id);
                TempData["SuccessMessage"] = "Đã xóa đánh giá thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Xóa đánh giá thất bại: {ex.Message}";
            }
            return RedirectToPage();
        }
    }
}