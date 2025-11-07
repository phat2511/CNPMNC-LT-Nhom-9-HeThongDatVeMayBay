namespace FlightAPI.Services.Dtos.Promotion
{
    public class PromotionReadDto
    {
        public int PromotionId { get; set; }
        public string? Code { get; set; } // (Mã có thể là NULL)
        public string Description { get; set; } = null!;

        // "Hàng" giảm
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountAmount { get; set; }

        // "Hạn" (Date)
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}