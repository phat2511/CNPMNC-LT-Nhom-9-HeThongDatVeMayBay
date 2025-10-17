using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class LoyaltyTransaction
{
    public int LoyaltyTransactionId { get; set; }

    public int AccountId { get; set; }

    public int PointsChange { get; set; }

    public string? Reason { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;
}
