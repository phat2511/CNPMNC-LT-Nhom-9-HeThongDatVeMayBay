using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace AirCloud_DA.Pages.Admin.Promotions
{
    public class IndexModel : PageModel
    {
        private readonly IPromotionService _promotions;

        public IndexModel(IPromotionService promotions)
        {
            _promotions = promotions;
        }

        public List<AirCloud_DA.Data.Promotion> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? Status { get; set; } = "All"; // All | Active | Inactive
        [BindProperty(SupportsGet = true)] public DateOnly? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateOnly? To { get; set; }
        [BindProperty(SupportsGet = true)] public bool OldestFirst { get; set; } = true;

        public async Task OnGetAsync()
        {
            bool? isActive = Status?.ToLower() switch
            {
                "active" => true,
                "inactive" => false,
                _ => (bool?)null
            };

            Items = await _promotions.GetPromotionsFilteredAsync(isActive, From, To, OldestFirst);
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var p = await _promotions.GetPromotionByIdAsync(id);
            if (p != null)
            {
                p.IsActive = !p.IsActive;
                await _promotions.UpdatePromotionAsync(p);
                TempData["SuccessMessage"] = p.IsActive ? "Đã bật ưu đãi." : "Đã tắt ưu đãi.";
            }

            return RedirectToPage(new
            {
                Status,
                From,
                To,
                OldestFirst
            });
        }
    }
}