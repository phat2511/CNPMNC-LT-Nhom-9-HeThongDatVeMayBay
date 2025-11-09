using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AirCloud_DA.Services;

namespace AirCloud_DA.Pages
{
    public class BookingModel : PageModel
    {
        private readonly IPromotionService _promotions;

        public BookingModel(IPromotionService promotions)
        {
            _promotions = promotions;
        }

        [BindProperty(SupportsGet = true)] public string Id { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string From { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string To { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public DateTime Date { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)] public int Passengers { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public string CabinClass { get; set; } = "Economy";

        [BindProperty] public string FullName { get; set; } = string.Empty;
        [BindProperty] public string Email { get; set; } = string.Empty;
        [BindProperty] public string Phone { get; set; } = string.Empty;
        [BindProperty] public string? PromoCode { get; set; }

        public FlightSearchResult? SelectedFlight { get; private set; }
        public AirCloud_DA.Data.Promotion? AppliedPromotion { get; private set; }
        public decimal OriginalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public string? SuccessMessage { get; private set; }

        public void OnGet()
        {
            SelectedFlight = RebuildFlightFromId(Id, From, To, Date);
            if (SelectedFlight != null)
            {
                OriginalPrice = SelectedFlight.Price;
                FinalPrice = OriginalPrice;
            }
        }

        public async Task OnPostAsync()
        {
            SelectedFlight = RebuildFlightFromId(Id, From, To, Date);
            
            if (!string.IsNullOrWhiteSpace(PromoCode))
            {
                AppliedPromotion = await _promotions.ValidatePromotionAsync(PromoCode, SelectedFlight?.Price ?? 0);
                if (AppliedPromotion != null)
                {
                    OriginalPrice = SelectedFlight?.Price ?? 0;
                    if (AppliedPromotion.DiscountPercent.HasValue)
                    {
                        FinalPrice = OriginalPrice * (1 - AppliedPromotion.DiscountPercent.Value / 100);
                    }
                    else if (AppliedPromotion.DiscountAmount.HasValue)
                    {
                        FinalPrice = Math.Max(0, OriginalPrice - AppliedPromotion.DiscountAmount.Value);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return;
            }

            SuccessMessage = $"Đặt vé thành công cho {FullName}. Mã đặt chỗ: {Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        private static FlightSearchResult RebuildFlightFromId(string id, string from, string to, DateTime date)
        {
            var seed = Math.Abs(HashCode.Combine(from?.ToUpperInvariant(), to?.ToUpperInvariant(), date.Date));
            var random = new Random(seed);

            var airlines = new[] { "VN", "VJ", "QH", "Bamboo" };
            var airline = airlines[random.Next(airlines.Length)];
            var baseDepart = date.Date.AddHours(6).AddMinutes(random.Next(0, 12) * 30);
            var durationMinutes = 90 + random.Next(0, 7) * 15;
            var arrive = baseDepart.AddMinutes(durationMinutes);
            var price = 900_000 + random.Next(0, 10) * 150_000;

            return new FlightSearchResult
            {
                FlightInstanceId = int.TryParse(id, out var flightId) ? flightId : 1,
                FlightNumber = $"{airline}{random.Next(100, 999)}",
                Airline = airline,
                FromAirport = $"{from} - {from} Airport",
                ToAirport = $"{to} - {to} Airport",
                DepartureTime = baseDepart,
                ArrivalTime = arrive,
                Price = price,
                Status = "Scheduled",
                Duration = $"{durationMinutes / 60}h {durationMinutes % 60}m"
            };
        }
    }
}