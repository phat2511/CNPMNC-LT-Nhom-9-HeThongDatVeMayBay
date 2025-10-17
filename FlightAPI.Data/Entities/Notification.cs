using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? AccountId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }
}
