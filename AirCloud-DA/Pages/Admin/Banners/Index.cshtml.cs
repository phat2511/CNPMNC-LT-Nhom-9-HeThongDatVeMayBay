using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace AirCloud_DA.Pages.Admin.Banners
{
    public class IndexModel : PageModel
    {
        private readonly IBannerService _banners;

        public IndexModel(IBannerService banners) { _banners = banners; }

        public List<AirCloud_DA.Data.Banner> Items { get; set; } = new();

        [BindProperty(SupportsGet = true)] public string? Status { get; set; } = "All"; // All | Active | Inactive
        [BindProperty(SupportsGet = true)] public DateTime? From { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? To { get; set; }
        [BindProperty(SupportsGet = true)] public bool OldestFirst { get; set; } = true;

        public async Task OnGetAsync()
        {
            bool? isActive = Status?.ToLower() switch
            {
                "active" => true,
                "inactive" => false,
                _ => (bool?)null
            };

            Items = await _banners.GetBannersFilteredAsync(isActive, From, To, OldestFirst);
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var b = await _banners.GetBannerByIdAsync(id);
            if (b != null)
            {
                b.IsActive = !b.IsActive;
                await _banners.UpdateBannerAsync(b);
                TempData["SuccessMessage"] = b.IsActive ? "Đã bật banner." : "Đã tắt banner.";
            }

            return RedirectToPage(new { Status, From, To, OldestFirst });
        }
    }
}