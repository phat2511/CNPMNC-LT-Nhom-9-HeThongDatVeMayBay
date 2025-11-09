using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class Seat
{
    public int SeatId { get; set; }

    public int FlightInstanceId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public int SeatClassId { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<BookingFlight> BookingFlights { get; set; } = new List<BookingFlight>();

    public virtual FlightInstance FlightInstance { get; set; } = null!;

    public virtual SeatClass SeatClass { get; set; } = null!;
}
