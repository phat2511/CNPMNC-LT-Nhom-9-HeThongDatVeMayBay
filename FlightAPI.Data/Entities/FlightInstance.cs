using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class FlightInstance
{
    public int FlightInstanceId { get; set; }

    public int FlightId { get; set; }

    public string DepartureAirport { get; set; } = null!;

    public string ArrivalAirport { get; set; } = null!;

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public decimal BasePrice { get; set; }

    public string? Status { get; set; }

    public virtual Airport ArrivalAirportNavigation { get; set; } = null!;

    public virtual ICollection<BookingFlight> BookingFlights { get; set; } = new List<BookingFlight>();

    public virtual Airport DepartureAirportNavigation { get; set; } = null!;

    public virtual Flight Flight { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
