using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class SeatClass
{
    public int SeatClassId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PriceMultiplier { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
