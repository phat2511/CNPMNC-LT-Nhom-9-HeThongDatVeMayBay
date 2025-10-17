using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public decimal? DiscountPercent { get; set; }

    public decimal? DiscountAmount { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
