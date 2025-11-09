using AirCloud_DA.Data;
using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Pages.Admin.Promotions
{
    public class CreateModel : PageModel
    {
        private readonly IPromotionService _promotions;

        public CreateModel(IPromotionService promotions)
        {
            _promotions = promotions;
        }

        [BindProperty] public Promotion Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                if (!string.IsNullOrWhiteSpace(Input.Code))
                {
                    var existed = await _promotions.GetPromotionByCodeAsync(Input.Code!);
                    if (existed != null)
                    {
                        ModelState.AddModelError("Input.Code", "Mã ưu đãi đã tồn tại.");
                        return Page();
                    }
                }

                if (Input.StartDate.HasValue && Input.EndDate.HasValue && Input.StartDate > Input.EndDate)
                {
                    ModelState.AddModelError("Input.EndDate", "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.");
                    return Page();
                }

                Input.IsActive = true;
                await _promotions.CreatePromotionAsync(Input);
                return RedirectToPage("Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"Không thể lưu khuyến mãi: {ex.InnerException?.Message ?? ex.Message}");
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