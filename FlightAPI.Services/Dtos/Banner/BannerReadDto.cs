namespace FlightAPI.Services.Dtos.Banner
{
    public class BannerReadDto
    {
        public int BannerId { get; set; }
        public string Title { get; set; } = null!;
        public string ImageUrl { get; set; } = null!; // Đường dẫn ảnh
        public string? LinkUrl { get; set; } // Link khi click
        public bool IsActive { get; set; }

        // "Hàng" bonus (móc vào "Deal" sếp vừa tạo)
        public int? PromotionId { get; set; }
        public string? PromotionDescription { get; set; } // "Giảm 10% mùa đông"
    }
}