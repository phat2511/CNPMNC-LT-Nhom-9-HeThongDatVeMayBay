using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class Otp
{
    public int OtpId { get; set; }

    public int? AccountId { get; set; }

    public string Destination { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Purpose { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }
}
