using AirCloud_DA.Data;
using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirCloud_DA.Pages.Admin.Promotions
{
    // Yêu cầu ID trong route để xác định đối tượng cần xóa
    public class DeleteModel : PageModel
    {
        private readonly IPromotionService _promotions;

        public DeleteModel(IPromotionService promotions)
        {
            _promotions = promotions;
        }

        [BindProperty]
        public Promotion Promotion { get; set; } = new();

        // 1. OnGetAsync: Tải thông tin mã ưu đãi dựa trên ID
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var p = await _promotions.GetPromotionByIdAsync(id);

            if (p == null)
            {
                // Nếu không tìm thấy, quay lại trang danh sách
                return RedirectToPage("./Index");
            }

            Promotion = p;
            return Page();
        }

        // 2. OnPostAsync: Thực hiện việc xóa
        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _promotions.DeletePromotionAsync(id);
                TempData["SuccessMessage"] = "Đã xóa ưu đãi thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Xóa ưu đãi thất bại: {ex.Message}";
            }
            return RedirectToPage("./Index");
        }
    }
}