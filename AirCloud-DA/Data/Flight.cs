using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class Flight
{
    public int FlightId { get; set; }

    public string FlightNumber { get; set; } = null!;

    public string AirlineCode { get; set; } = null!;

    public virtual Airline AirlineCodeNavigation { get; set; } = null!;

    public virtual ICollection<FlightInstance> FlightInstances { get; set; } = new List<FlightInstance>();
}
