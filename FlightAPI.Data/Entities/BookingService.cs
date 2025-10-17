using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class BookingService
{
    public int BookingServiceId { get; set; }

    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
