using Microsoft.AspNetCore.Identity; // <--- THÊM BẮT BUỘC
using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

// THAY ĐỔI LỚN: Kế thừa từ IdentityUser<int> (vì key của bạn là INT)
public partial class Account : IdentityUser<int>
{
    // [BỊ LOẠI BỎ] - IdentityUser đã có "Id" (kiểu int)
    // public int AccountId { get; set; }

    // [BỊ LOẠI BỎ] - IdentityUser đã có "UserName" (chúng ta sẽ map nó với "Username" trong DbContext)
    // public string Username { get; set; } = null!;

    // [BỊ LOẠI BỎ] - IdentityUser đã có "PasswordHash"
    // public string PasswordHash { get; set; } = null!;

    // [GIỮ LẠI] - Đây là trường custom của bạn
    public string FullName { get; set; } = null!;

    // [BỊ LOẠI BỎ] - IdentityUser đã có "Email"
    // public string Email { get; set; } = null!;

    // [BỊ LOẠI BỎ] - IdentityUser đã có "PhoneNumber" (chúng ta sẽ map nó với "Phone")
    // public string? Phone { get; set; }

    // [THAY ĐỔI CỰC KỲ QUAN TRỌNG]
    // Identity quản lý vai trò (Role) qua bảng UserRoles (cho phép 1 user có NHIỀU role).
    // Chúng ta phải loại bỏ "RoleId" và "Role" ở đây.
    // Đừng lo, ta đã cấu hình việc này trong DbContext ở bước 2.2 (hướng dẫn trước).
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;


    // [GIỮ LẠI] - Đây là trường custom của bạn
    public DateTime? CreatedAt { get; set; }

    // [BỊ LOẠI BỎ] - IdentityUser đã có "EmailConfirmed" (kiểu bool)
    // public bool? IsEmailConfirmed { get; set; }

    // [GIỮ LẠI] - Đây là trường custom của bạn (dùng cho Admin khoá)
    // Identity có "LockoutEnd" (dùng cho khoá tự động), 
    // nên "IsLocked" của bạn vẫn hữu ích.
    public bool? IsLocked { get; set; }

    // [GIỮ LẠI] - Custom
    public int? LoyaltyPoints { get; set; }

    // [GIỮ LẠI] - Custom
    public int? MembershipTierId { get; set; }

    // [GIỮ LẠI] - Custom
    public string? LockedBy { get; set; }

    // [GIỮ LẠI] - Custom
    public DateTime? LockedAt { get; set; }

    // [GIỮ LẠI] - Custom
    public string? LockReason { get; set; }

    // [GIỮ LẠI] - Tất cả các ICollection (navigation properties) đều giữ lại
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    public virtual ICollection<LoyaltyTransaction> LoyaltyTransactions { get; set; } = new List<LoyaltyTransaction>();

    public virtual MembershipTier? MembershipTier { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

    // Bạn có TicketMessages ở đây nhưng không có trong schema ban đầu,
    // tôi cứ giữ lại, bạn tự xem xét nhé.
    public virtual ICollection<TicketMessage> TicketMessages { get; set; } = new List<TicketMessage>();
}