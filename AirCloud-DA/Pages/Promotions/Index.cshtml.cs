using AirCloud_DA.Data;
using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirCloud_DA.Pages.Promotions
{
    public class IndexModel : PageModel
    {
        private readonly IPromotionService _promotions;

        public IndexModel(IPromotionService promotions)
        {
            _promotions = promotions;
        }

        public List<Promotion> Items { get; set; } = new();

        public async Task OnGetAsync()
        {
            Items = await _promotions.GetActivePromotionsAsync();
        }
    }
}
