using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class Airline
{
    public string AirlineCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Website { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
