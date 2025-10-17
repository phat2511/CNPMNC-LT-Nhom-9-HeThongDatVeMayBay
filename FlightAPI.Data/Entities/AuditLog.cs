using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class AuditLog
{
    public long AuditId { get; set; }

    public int? AccountId { get; set; }

    public string? Action { get; set; }

    public string? Entity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }
}
