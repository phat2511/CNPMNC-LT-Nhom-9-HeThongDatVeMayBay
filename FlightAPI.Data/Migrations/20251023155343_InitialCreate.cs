using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.EnsureSchema(
                name: "core");

            migrationBuilder.EnsureSchema(
                name: "admin");

            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.CreateTable(
                name: "Airline",
                schema: "core",
                columns: table => new
                {
                    AirlineCode = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Airline__79E77E120054A031", x => x.AirlineCode);
                });

            migrationBuilder.CreateTable(
                name: "Airport",
                schema: "core",
                columns: table => new
                {
                    AirportCode = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Airport__4B677352EDD78E13", x => x.AirportCode);
                });

            migrationBuilder.CreateTable(
                name: "MembershipTier",
                schema: "admin",
                columns: table => new
                {
                    MembershipTierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinPoints = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    Benefits = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Membersh__B741E0306B3AEB63", x => x.MembershipTierId);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                schema: "core",
                columns: table => new
                {
                    PromotionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Promotio__52C42FCF3D03AE70", x => x.PromotionId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE1A3DF201D6", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "SeatClass",
                schema: "core",
                columns: table => new
                {
                    SeatClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PriceMultiplier = table.Column<decimal>(type: "decimal(5,2)", nullable: true, defaultValue: 1.0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SeatClas__41D153A8550691B8", x => x.SeatClassId);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                schema: "sales",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Service__C51BB00AD02BFCC9", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                schema: "core",
                columns: table => new
                {
                    FlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AirlineCode = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Flight__8A9E14EE3874E902", x => x.FlightId);
                    table.ForeignKey(
                        name: "FK__Flight__AirlineC__72C60C4A",
                        column: x => x.AirlineCode,
                        principalSchema: "core",
                        principalTable: "Airline",
                        principalColumn: "AirlineCode");
                });

            migrationBuilder.CreateTable(
                name: "Account",
                schema: "auth",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    LoyaltyPoints = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    MembershipTierId = table.Column<int>(type: "int", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__349DA5A6C830A2B1", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Account_MembershipTier",
                        column: x => x.MembershipTierId,
                        principalSchema: "admin",
                        principalTable: "MembershipTier",
                        principalColumn: "MembershipTierId");
                    table.ForeignKey(
                        name: "FK__Account__RoleId__4F7CD00D",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "FlightInstance",
                schema: "core",
                columns: table => new
                {
                    FlightInstanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    DepartureAirport = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                    ArrivalAirport = table.Column<string>(type: "char(5)", unicode: false, fixedLength: true, maxLength: 5, nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Scheduled")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FlightIn__FD0BBB3109636797", x => x.FlightInstanceId);
                    table.ForeignKey(
                        name: "FK__FlightIns__Arriv__797309D9",
                        column: x => x.ArrivalAirport,
                        principalSchema: "core",
                        principalTable: "Airport",
                        principalColumn: "AirportCode");
                    table.ForeignKey(
                        name: "FK__FlightIns__Depar__787EE5A0",
                        column: x => x.DepartureAirport,
                        principalSchema: "core",
                        principalTable: "Airport",
                        principalColumn: "AirportCode");
                    table.ForeignKey(
                        name: "FK__FlightIns__Fligh__778AC167",
                        column: x => x.FlightId,
                        principalSchema: "core",
                        principalTable: "Flight",
                        principalColumn: "FlightId");
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "admin",
                columns: table => new
                {
                    AuditId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Entity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AuditLog__A17F23981B3E6A1F", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK__AuditLog__Accoun__3493CFA7",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "Banner",
                schema: "core",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LinkUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    PromotionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Banner__32E86AD17C3683F2", x => x.BannerId);
                    table.ForeignKey(
                        name: "FK_Banner_Account",
                        column: x => x.CreatedBy,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_Banner_Promotion",
                        column: x => x.PromotionId,
                        principalSchema: "core",
                        principalTable: "Promotion",
                        principalColumn: "PromotionId");
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                schema: "sales",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BookingStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    PromotionId = table.Column<int>(type: "int", nullable: true),
                    DiscountAmountApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Booking__73951AED0577A144", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Booking_Promotion",
                        column: x => x.PromotionId,
                        principalSchema: "core",
                        principalTable: "Promotion",
                        principalColumn: "PromotionId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__Booking__Account__1332DBDC",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "EmailVerification",
                schema: "auth",
                columns: table => new
                {
                    VerificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailVer__306D49076E06154D", x => x.VerificationId);
                    table.ForeignKey(
                        name: "FK__EmailVeri__Accou__571DF1D5",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "LoyaltyTransaction",
                schema: "sales",
                columns: table => new
                {
                    LoyaltyTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    PointsChange = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoyaltyT__5CFDF9F455CF2087", x => x.LoyaltyTransactionId);
                    table.ForeignKey(
                        name: "FK__LoyaltyTr__Accou__2645B050",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "admin",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E12A6425561", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK__Notificat__Accou__30C33EC3",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "OTP",
                schema: "auth",
                columns: table => new
                {
                    OtpId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Destination = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OTP__3143C4A37D8C7E06", x => x.OtpId);
                    table.ForeignKey(
                        name: "FK__OTP__AccountId__5BE2A6F2",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "SupportTicket",
                schema: "admin",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Open"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SupportT__712CC60737684DEA", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK__SupportTi__Accou__2BFE89A6",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                });

            migrationBuilder.CreateTable(
                name: "Seat",
                schema: "core",
                columns: table => new
                {
                    SeatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightInstanceId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SeatClassId = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seat__311713F3E89F104E", x => x.SeatId);
                    table.ForeignKey(
                        name: "FK__Seat__FlightInst__05D8E0BE",
                        column: x => x.FlightInstanceId,
                        principalSchema: "core",
                        principalTable: "FlightInstance",
                        principalColumn: "FlightInstanceId");
                    table.ForeignKey(
                        name: "FK__Seat__SeatClassI__06CD04F7",
                        column: x => x.SeatClassId,
                        principalSchema: "core",
                        principalTable: "SeatClass",
                        principalColumn: "SeatClassId");
                });

            migrationBuilder.CreateTable(
                name: "BookingService",
                schema: "sales",
                columns: table => new
                {
                    BookingServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingS__43F55CB1FE23BA42", x => x.BookingServiceId);
                    table.ForeignKey(
                        name: "FK_BookingService_Booking",
                        column: x => x.BookingId,
                        principalSchema: "sales",
                        principalTable: "Booking",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingService_Service",
                        column: x => x.ServiceId,
                        principalSchema: "sales",
                        principalTable: "Service",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "sales",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A38FC62BE38", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK__Payment__Booking__22751F6C",
                        column: x => x.BookingId,
                        principalSchema: "sales",
                        principalTable: "Booking",
                        principalColumn: "BookingId");
                });

            migrationBuilder.CreateTable(
                name: "TicketMessage",
                schema: "admin",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    MessageContent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    IsStaffReply = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    SenderRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TicketMe__C87C0C9CC3731D28", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK__TicketMes__Accou__489AC854",
                        column: x => x.AccountId,
                        principalSchema: "auth",
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK__TicketMes__Ticke__47A6A41B",
                        column: x => x.TicketId,
                        principalSchema: "admin",
                        principalTable: "SupportTicket",
                        principalColumn: "TicketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingFlight",
                schema: "sales",
                columns: table => new
                {
                    BookingFlightId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    FlightInstanceId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: true),
                    PassengerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PassengerType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Fare = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingF__E29A261456EF46D0", x => x.BookingFlightId);
                    table.ForeignKey(
                        name: "FK__BookingFl__Booki__17F790F9",
                        column: x => x.BookingId,
                        principalSchema: "sales",
                        principalTable: "Booking",
                        principalColumn: "BookingId");
                    table.ForeignKey(
                        name: "FK__BookingFl__Fligh__18EBB532",
                        column: x => x.FlightInstanceId,
                        principalSchema: "core",
                        principalTable: "FlightInstance",
                        principalColumn: "FlightInstanceId");
                    table.ForeignKey(
                        name: "FK__BookingFl__SeatI__19DFD96B",
                        column: x => x.SeatId,
                        principalSchema: "core",
                        principalTable: "Seat",
                        principalColumn: "SeatId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_MembershipTierId",
                schema: "auth",
                table: "Account",
                column: "MembershipTierId");

            migrationBuilder.CreateIndex(
                name: "IX_Account_RoleId",
                schema: "auth",
                table: "Account",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__536C85E430E6D1B2",
                schema: "auth",
                table: "Account",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Account__5C7E359E3D9F4634",
                schema: "auth",
                table: "Account",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__A9D10534FB30B529",
                schema: "auth",
                table: "Account",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_AccountId",
                schema: "admin",
                table: "AuditLog",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_CreatedBy",
                schema: "core",
                table: "Banner",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_PromotionId",
                schema: "core",
                table: "Banner",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_AccountId",
                schema: "sales",
                table: "Booking",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_PromotionId",
                schema: "sales",
                table: "Booking",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "UQ__Booking__C6E56BD5EE7BB564",
                schema: "sales",
                table: "Booking",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlight_BookingId",
                schema: "sales",
                table: "BookingFlight",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlight_FlightInstanceId",
                schema: "sales",
                table: "BookingFlight",
                column: "FlightInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingFlight_SeatId",
                schema: "sales",
                table: "BookingFlight",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_ServiceId",
                schema: "sales",
                table: "BookingService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "UQ_Booking_Service",
                schema: "sales",
                table: "BookingService",
                columns: new[] { "BookingId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerification_AccountId",
                schema: "auth",
                table: "EmailVerification",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_AirlineCode",
                schema: "core",
                table: "Flight",
                column: "AirlineCode");

            migrationBuilder.CreateIndex(
                name: "UQ__Flight__2EAE6F502D1C4F7F",
                schema: "core",
                table: "Flight",
                column: "FlightNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlightInstance_ArrivalAirport",
                schema: "core",
                table: "FlightInstance",
                column: "ArrivalAirport");

            migrationBuilder.CreateIndex(
                name: "IX_FlightInstance_DepartureAirport",
                schema: "core",
                table: "FlightInstance",
                column: "DepartureAirport");

            migrationBuilder.CreateIndex(
                name: "IX_FlightInstance_FlightId",
                schema: "core",
                table: "FlightInstance",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_LoyaltyTransaction_AccountId",
                schema: "sales",
                table: "LoyaltyTransaction",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AccountId",
                schema: "admin",
                table: "Notification",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OTP_AccountId",
                schema: "auth",
                table: "OTP",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingId",
                schema: "sales",
                table: "Payment",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "UQ__Promotio__A25C5AA76B7F25D0",
                schema: "core",
                table: "Promotion",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Role__8A2B6160CED64AE4",
                schema: "auth",
                table: "Role",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seat_SeatClassId",
                schema: "core",
                table: "Seat",
                column: "SeatClassId");

            migrationBuilder.CreateIndex(
                name: "UQ_Seat_Flight",
                schema: "core",
                table: "Seat",
                columns: new[] { "FlightInstanceId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportTicket_AccountId",
                schema: "admin",
                table: "SupportTicket",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessage_AccountId",
                schema: "admin",
                table: "TicketMessage",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessage_TicketId",
                schema: "admin",
                table: "TicketMessage",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "Banner",
                schema: "core");

            migrationBuilder.DropTable(
                name: "BookingFlight",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "BookingService",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "EmailVerification",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "LoyaltyTransaction",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "OTP",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "TicketMessage",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "Seat",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Service",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "Booking",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "SupportTicket",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "FlightInstance",
                schema: "core");

            migrationBuilder.DropTable(
                name: "SeatClass",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Promotion",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Account",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Airport",
                schema: "core");

            migrationBuilder.DropTable(
                name: "Flight",
                schema: "core");

            migrationBuilder.DropTable(
                name: "MembershipTier",
                schema: "admin");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Airline",
                schema: "core");
        }
    }
}
