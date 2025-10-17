using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class SupportTicket
{
    public int TicketId { get; set; }

    public int? AccountId { get; set; }

    public string? Subject { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<TicketMessage> TicketMessages { get; set; } = new List<TicketMessage>();
}
