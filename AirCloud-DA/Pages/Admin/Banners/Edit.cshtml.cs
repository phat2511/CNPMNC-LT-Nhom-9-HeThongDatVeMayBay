using AirCloud_DA.Data;
using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Pages.Admin.Banners
{
    public class EditModel : PageModel
    {
        private readonly IBannerService _banners;
        public EditModel(IBannerService banners) { _banners = banners; }

        [BindProperty] public Banner Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var b = await _banners.GetBannerByIdAsync(id);
            if (b == null) return RedirectToPage("Index");
            Input = b;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (string.IsNullOrWhiteSpace(Input.Title))
                ModelState.AddModelError("Input.Title", "Tiêu đề là bắt buộc.");
            if (string.IsNullOrWhiteSpace(Input.ImageUrl))
                ModelState.AddModelError("Input.ImageUrl", "Đường dẫn ảnh là bắt buộc.");
            if (Input.StartDate.HasValue && Input.EndDate.HasValue && Input.StartDate > Input.EndDate)
                ModelState.AddModelError("Input.EndDate", "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");

            if (!ModelState.IsValid) return Page();

            try
            {
                await _banners.UpdateBannerAsync(Input);
                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"Không thể cập nhật banner: {ex.InnerException?.Message ?? ex.Message}");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
                return Page();
            }
        }
    }
}