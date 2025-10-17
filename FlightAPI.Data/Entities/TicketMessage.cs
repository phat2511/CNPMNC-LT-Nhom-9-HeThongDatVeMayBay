using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class TicketMessage
{
    public int MessageId { get; set; }

    public int TicketId { get; set; }

    public int? AccountId { get; set; }

    public string MessageContent { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    public bool? IsStaffReply { get; set; }

    public string SenderRole { get; set; } = null!;

    public virtual Account? Account { get; set; }

    public virtual SupportTicket Ticket { get; set; } = null!;
}
