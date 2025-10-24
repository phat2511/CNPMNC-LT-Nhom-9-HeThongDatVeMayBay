using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Data.Entities;

public partial class Airline
{
    [Required]
    [StringLength(3)]
    public string AirlineCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Website { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
