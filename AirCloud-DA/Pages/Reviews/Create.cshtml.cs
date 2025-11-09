using AirCloud_DA.Data;
using AirCloud_DA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirCloud_DA.Pages.Reviews
{
    public class CreateModel : PageModel
    {
        private readonly ISupportService _support;

        public CreateModel(ISupportService support)
        {
            _support = support;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        public int Rating { get; set; } = 0; // Đổi default về 0 để bắt buộc phải chọn

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
            [StringLength(300, ErrorMessage = "Tiêu đề không được vượt quá 300 ký tự")]
            public string? Subject { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
            [StringLength(2000, ErrorMessage = "Nội dung không được vượt quá 2000 ký tự")]
            public string? Content { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // Debug: Log Rating value
            Console.WriteLine($"Rating received: {Rating}");

            // Validate Rating
            if (Rating < 1 || Rating > 5)
            {
                ModelState.AddModelError("Rating", "Vui lòng chọn số sao từ 1 đến 5");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Đảm bảo Rating hợp lệ
                if (Rating < 1 || Rating > 5)
                {
                    ModelState.AddModelError("Rating", "Đánh giá phải từ 1 đến 5 sao");
                    return Page();
                }

                // Tạo SupportTicket từ Input với format chuẩn
                var ticket = new SupportTicket
                {
                    // Format Subject: "Đánh giá X sao - [Tiêu đề người dùng nhập]"
                    Subject = $"Đánh giá {Rating} sao - {Input.Subject?.Trim()}",

                    // Format Content: "[Đánh giá: X/5 sao]\n\n[Nội dung người dùng nhập]"
                    Content = $"[Đánh giá: {Rating}/5 sao]{Environment.NewLine}{Environment.NewLine}{Input.Content?.Trim()}",

                    Status = "Resolved",
                    AccountId = null,
                    CreatedAt = DateTime.Now
                };

                // Debug: Log ticket data
                Console.WriteLine($"Creating review - Subject: {ticket.Subject}");
                Console.WriteLine($"Creating review - Content: {ticket.Content}");

                await _support.CreateReviewAsync(ticket);

                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi thành công!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                Console.WriteLine($"Error creating review: {ex.Message}");
                return Page();
            }
        }
    }
}