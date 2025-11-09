using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class MembershipTier
{
    public int MembershipTierId { get; set; }

    public string Name { get; set; } = null!;

    public int? MinPoints { get; set; }

    public string? Benefits { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
