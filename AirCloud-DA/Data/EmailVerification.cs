using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class EmailVerification
{
    public int VerificationId { get; set; }

    public int AccountId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account Account { get; set; } = null!;
}
