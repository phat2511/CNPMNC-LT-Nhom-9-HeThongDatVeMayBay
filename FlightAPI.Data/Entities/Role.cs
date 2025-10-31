using Microsoft.AspNetCore.Identity; // <--- THÊM BẮT BUỘC
using System;
using System.Collections.Generic;

namespace FlightAPI.Data.Entities;

// THAY ĐỔI LỚN: Kế thừa từ IdentityRole<int>
public partial class Role : IdentityRole<int>
{
    // [BỊ LOẠI BỎ] - IdentityRole đã có "Id" (kiểu int)
    // public int RoleId { get; set; }

    // [BỊ LOẠI BỎ] - IdentityRole đã có "Name" (chúng ta sẽ map nó với "RoleName")
    // public string RoleName { get; set; } = null!;

    // [BỊ LOẠI BỎ] - CỰC KỲ QUAN TRỌNG
    // Identity quản lý quan hệ User-Role qua bảng trung gian (UserRoles).
    // Giữ lại collection này sẽ gây xung đột.
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}