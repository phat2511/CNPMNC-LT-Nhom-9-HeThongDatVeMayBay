using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class BookingFlight
{
    public int BookingFlightId { get; set; }

    public int BookingId { get; set; }

    public int FlightInstanceId { get; set; }

    public int? SeatId { get; set; }

    public string PassengerName { get; set; } = null!;

    public string? PassengerType { get; set; }

    public decimal? Fare { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual FlightInstance FlightInstance { get; set; } = null!;

    public virtual Seat? Seat { get; set; }
}
