// (Sếp phải 'using System.ComponentModel.DataAnnotations;' ở đầu)
using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Promotion
{
    // "Tờ khai" này "thông minh", nó "tự check" logic (IValidatableObject)
    public class PromotionRequestDto : IValidatableObject
    {
        [StringLength(50)]
        public string? Code { get; set; } // (Mã có thể là NULL, ví dụ: "Sale 11/11")

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        // === "CẶP ĐÔI NGUY HIỂM" (Chỉ được chọn 1) ===
        [Range(0, 100)]
        public decimal? DiscountPercent { get; set; } // "Giảm 10%" (null nếu giảm tiền)

        [Range(0, 99999999)]
        public decimal? DiscountAmount { get; set; } // "Giảm 50k" (null nếu giảm %)
        // ===========================================

        // (Dùng DateOnly cho "chuẩn" (vì DB của sếp là DATE))
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // === "BỘ NÃO" VALIDATE (Kiểm tra logic chéo) ===
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. Check "Cặp đôi" (Percent vs Amount)
            if (DiscountPercent.HasValue && DiscountAmount.HasValue)
            {
                // Sếp "tham", chọn cả 2
                yield return new ValidationResult(
                    "Không thể giảm cả % và số tiền. Vui lòng chỉ chọn 1.",
                    new[] { nameof(DiscountPercent), nameof(DiscountAmount) });
            }
            if (!DiscountPercent.HasValue && !DiscountAmount.HasValue)
            {
                // Sếp "lười", không chọn cái nào (Khuyến mãi "0 đồng"?)
                yield return new ValidationResult(
                   "Phải nhập % giảm hoặc số tiền giảm (ít nhất 1 trong 2).",
                   new[] { nameof(DiscountPercent), nameof(DiscountAmount) });
            }

            // 2. Check "Hạn" (Date)
            if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.",
                    new[] { nameof(StartDate), nameof(EndDate) });
            }
        }
    }
}