using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace AirCloud_DA.Pages.Admin.Reports
{
    public class RevenueModel : PageModel
    {
        private readonly IRevenueService _revenue;

        public RevenueModel(IRevenueService revenue)
        {
            _revenue = revenue;
        }

        [BindProperty(SupportsGet = true)] public DateTime From { get; set; } = DateTime.Today.AddDays(-7);
        [BindProperty(SupportsGet = true)] public DateTime To { get; set; } = DateTime.Today;
        [BindProperty(SupportsGet = true)] public string Period { get; set; } = "Day"; // Day | Week | Month

        public RevenueSummary Summary { get; private set; } = new();
        public List<RevenueReport> Series { get; private set; } = new();

        public async Task OnGetAsync()
        {
            var fromUtc = From;
            var toUtc = To.AddDays(1).AddTicks(-1);
            var daily = await _revenue.GetRevenueReportAsync(fromUtc, toUtc);
            Series = GroupByPeriod(daily, Period);
            Summary = await _revenue.GetRevenueSummaryAsync(fromUtc, toUtc);
        }

        private static List<RevenueReport> GroupByPeriod(IEnumerable<RevenueReport> daily, string period)
        {
            period = period?.Trim() ?? "Day";
            if (period.Equals("Week", StringComparison.OrdinalIgnoreCase))
            {
                var cal = CultureInfo.CurrentCulture.Calendar;
                var weekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                var firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                return daily
                    .GroupBy(d => new
                    {
                        Year = d.Date.Year,
                        Week = cal.GetWeekOfYear(d.Date, weekRule, firstDay)
                    })
                    .Select(g => new RevenueReport
                    {
                        // Use the Monday of that week as representative date
                        Date = FirstDateOfWeek(g.Key.Year, g.Key.Week, firstDay),
                        TotalRevenue = g.Sum(x => x.TotalRevenue),
                        TotalBookings = g.Sum(x => x.TotalBookings),
                        AverageOrderValue = g.Sum(x => x.TotalRevenue) / Math.Max(1, g.Sum(x => x.TotalBookings))
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
            }
            else if (period.Equals("Month", StringComparison.OrdinalIgnoreCase))
            {
                return daily
                    .GroupBy(d => new { d.Date.Year, d.Date.Month })
                    .Select(g => new RevenueReport
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        TotalRevenue = g.Sum(x => x.TotalRevenue),
                        TotalBookings = g.Sum(x => x.TotalBookings),
                        AverageOrderValue = g.Sum(x => x.TotalRevenue) / Math.Max(1, g.Sum(x => x.TotalBookings))
                    })
                    .OrderBy(x => x.Date)
                    .ToList();
            }
            else
            {
                return daily.OrderBy(x => x.Date).ToList();
            }
        }

        private static DateTime FirstDateOfWeek(int year, int weekOfYear, DayOfWeek firstDayOfWeek)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (firstDayOfWeek - jan1.DayOfWeek + 7) % 7;
            DateTime firstWeekStart = jan1.AddDays(daysOffset);
            return firstWeekStart.AddDays((weekOfYear - 1) * 7);
        }

        public async Task<FileContentResult> OnGetExportCsvAsync()
        {
            var fromUtc = From;
            var toUtc = To.AddDays(1).AddTicks(-1);
            var daily = await _revenue.GetRevenueReportAsync(fromUtc, toUtc);
            var series = GroupByPeriod(daily, Period);

            var culture = CultureInfo.GetCultureInfo("vi-VN");
            var lines = new List<string> { "Period,TotalRevenue,TotalBookings,AverageOrderValue" };
            foreach (var r in series)
            {
                string periodLabel = Period switch
                {
                    "Week" => $"Tuần {ISOWeek.GetWeekOfYear(r.Date)}, {r.Date:yyyy}",
                    "Month" => r.Date.ToString("MM/yyyy"),
                    _ => r.Date.ToString("dd/MM/yyyy")
                };
                lines.Add(string.Join(',',
                    Escape(periodLabel),
                    r.TotalRevenue.ToString(culture),
                    r.TotalBookings.ToString(culture),
                    r.AverageOrderValue.ToString(culture)));
            }
            var csv = string.Join("\r\n", lines);
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", $"revenue_{From:yyyyMMdd}_{To:yyyyMMdd}_{Period}.csv");
        }

        private static string Escape(string s) => s.Contains(',') ? $"\"{s.Replace("\"", "\"\"")}\"" : s;

        public async Task<FileContentResult> OnGetExportPdfAsync()
        {
            var fromUtc = From;
            var toUtc = To.AddDays(1).AddTicks(-1);
            var daily = await _revenue.GetRevenueReportAsync(fromUtc, toUtc);
            var series = GroupByPeriod(daily, Period);
            var summary = await _revenue.GetRevenueSummaryAsync(fromUtc, toUtc);

            // Build PDF using QuestPDF
            var doc = BuildRevenuePdf(series, summary, From, To, Period);
            var pdf = doc.GeneratePdf();
            return File(pdf, "application/pdf", $"revenue_{From:yyyyMMdd}_{To:yyyyMMdd}_{Period}.pdf");
        }

        private static IDocument BuildRevenuePdf(IEnumerable<RevenueReport> series, RevenueSummary summary, DateTime from, DateTime to, string period)
        {
            var culture = CultureInfo.GetCultureInfo("vi-VN");
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Text($"Báo cáo doanh thu ({period})").SemiBold().FontSize(16);
                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text($"Khoảng thời gian: {from:dd/MM/yyyy} - {to:dd/MM/yyyy}");
                        col.Item().Text($"Tổng doanh thu: {summary.TotalRevenue.ToString("C0", culture)}");
                        col.Item().Text($"Số đơn: {summary.TotalBookings}");
                        col.Item().Text($"Giá trị TB: {summary.AverageOrderValue.ToString("C0", culture)}");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });
                            table.Header(header =>
                            {
                                header.Cell().Element(CellHeader).Text("Kỳ");
                                header.Cell().Element(CellHeader).Text("Doanh thu");
                                header.Cell().Element(CellHeader).Text("Số đơn");
                                header.Cell().Element(CellHeader).Text("Giá trị TB");
                            });

                            foreach (var r in series)
                            {
                                var label = period switch
                                {
                                    "Week" => $"Tuần {ISOWeek.GetWeekOfYear(r.Date)}, {r.Date:yyyy}",
                                    "Month" => r.Date.ToString("MM/yyyy"),
                                    _ => r.Date.ToString("dd/MM/yyyy")
                                };
                                table.Cell().Text(label);
                                table.Cell().Text(r.TotalRevenue.ToString("C0", culture)).AlignRight();
                                table.Cell().Text(r.TotalBookings.ToString(culture)).AlignRight();
                                table.Cell().Text(r.AverageOrderValue.ToString("C0", culture)).AlignRight();
                            }
                        });
                    });

                    static IContainer CellHeader(IContainer c) => c.DefaultTextStyle(x => x.SemiBold());
                });
            });
        }
    }
}