using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirCloud_DA.Pages
{
    public class ResultsModel : PageModel
    {
        private readonly IFlightSearchService _search;

        public ResultsModel(IFlightSearchService search)
        {
            _search = search;
        }

        [BindProperty(SupportsGet = true)] public string From { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public string To { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)] public DateTime Date { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)] public string? ReturnDate { get; set; } // yyyy-MM-dd hoặc null
        [BindProperty(SupportsGet = true)] public int Passengers { get; set; } = 1;
        [BindProperty(SupportsGet = true)] public string CabinClass { get; set; } = "Economy";

        public List<FlightSearchResult> Flights { get; private set; } = new();

        public async Task OnGetAsync()
        {
            DateTime? rd = null;
            if (!string.IsNullOrWhiteSpace(ReturnDate) && DateTime.TryParse(ReturnDate, out var rdt))
                rd = rdt;

            Flights = await _search.SearchFlightsAsync(From, To, Date, rd, Passengers, CabinClass);
        }
    }
}
