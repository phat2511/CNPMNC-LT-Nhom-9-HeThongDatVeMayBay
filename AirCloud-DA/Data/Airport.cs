using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class Airport
{
    public string AirportCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<FlightInstance> FlightInstanceArrivalAirportNavigations { get; set; } = new List<FlightInstance>();

    public virtual ICollection<FlightInstance> FlightInstanceDepartureAirportNavigations { get; set; } = new List<FlightInstance>();
}
