using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class SupportTicket
{
    public int TicketId { get; set; }

    public int? AccountId { get; set; }

    public string? Subject { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }
}
