using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public decimal? Price { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}
