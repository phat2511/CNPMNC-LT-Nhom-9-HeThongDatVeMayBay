using AirCloud_DA.Data; // Hoặc namespace chứa lớp Banner
using AirCloud_DA.Services; // Thay bằng namespace chứa dịch vụ Banner của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirCloud_DA.Pages.Admin.Banners
{
    // Giả định bạn có IBannerService và Banner model
    public class DeleteModel : PageModel
    {
        private readonly IBannerService _banners;

        public DeleteModel(IBannerService banners)
        {
            _banners = banners;
        }

        [BindProperty]
        public Banner Banner { get; set; } = new(); // Thay Banner bằng Banner? nếu bạn dùng Nullable Reference Types

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // GIẢ ĐỊNH: Dịch vụ của bạn có GetBannerByIdAsync
            var b = await _banners.GetBannerByIdAsync(id);

            if (b == null) return RedirectToPage("./Index");

            // Tùy chọn: Dùng Banner = b!; nếu Banner là non-nullable
            // hoặc thay đổi public Banner Banner thành public Banner? Banner
            Banner = b;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _banners.DeleteBannerAsync(id);
                TempData["SuccessMessage"] = "Đã xóa banner thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Xóa banner thất bại: {ex.Message}";
            }
            return RedirectToPage("./Index");
        }
    }
}