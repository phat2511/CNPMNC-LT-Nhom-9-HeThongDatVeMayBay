using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightAPI.Data.Entities;

public partial class Flight
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FlightId { get; set; }

    public string FlightNumber { get; set; } = null!;

    public string AirlineCode { get; set; } = null!;

    public virtual Airline AirlineCodeNavigation { get; set; } = null!;

    public virtual ICollection<FlightInstance> FlightInstances { get; set; } = new List<FlightInstance>();
}
