using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class Booking
{
    public int BookingId { get; set; }

    public string BookingCode { get; set; } = null!;

    public int? AccountId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? BookingStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? PromotionId { get; set; }

    public decimal? DiscountAmountApplied { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<BookingFlight> BookingFlights { get; set; } = new List<BookingFlight>();

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Promotion? Promotion { get; set; }
}
