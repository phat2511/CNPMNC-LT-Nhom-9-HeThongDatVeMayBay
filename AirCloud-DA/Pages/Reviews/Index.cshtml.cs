using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

// Import đầy đủ namespace cho SupportTicket
using AirCloud_DA.Data;

namespace AirCloud_DA.Pages.Reviews
{
    public class IndexModel : PageModel
    {
        private readonly ISupportService _support;

        public IndexModel(ISupportService support)
        {
            _support = support;
        }

        // Thuộc tính để lưu trữ danh sách đánh giá được hiển thị trên trang
        public List<SupportTicket> Reviews { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Lấy tất cả đánh giá đã được duyệt từ SupportService
            Reviews = await _support.GetAllReviewsAsync();
        }
    }
}