using System;
using System.Collections.Generic;

namespace AirCloud_DA.Data;

public partial class Banner
{
    public int BannerId { get; set; }

    public string Title { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? PromotionId { get; set; }

    public virtual Account? CreatedByNavigation { get; set; }

    public virtual Promotion? Promotion { get; set; }
}
