using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Data;

public partial class AirCloudDbContext : DbContext
{
    public AirCloudDbContext()
    {
    }

    public AirCloudDbContext(DbContextOptions<AirCloudDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Airline> Airlines { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingFlight> BookingFlights { get; set; }

    public virtual DbSet<BookingService> BookingServices { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightInstance> FlightInstances { get; set; }

    public virtual DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }

    public virtual DbSet<MembershipTier> MembershipTiers { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<SeatClass> SeatClasses { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<SupportTicket> SupportTickets { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A697F3135E");

            entity.ToTable("Account", "auth");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E48F314C07").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Account__5C7E359EECD081A9").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Account__A9D10534F147CA27").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsEmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.IsLocked).HasDefaultValue(false);
            entity.Property(e => e.LockReason).HasMaxLength(255);
            entity.Property(e => e.LockedBy).HasMaxLength(20);
            entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.MembershipTier).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.MembershipTierId)
                .HasConstraintName("FK_Account_MembershipTier");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__RoleId__3D5E1FD2");
        });

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.HasKey(e => e.AirlineCode).HasName("PK__Airline__79E77E12F2F9D9FE");

            entity.ToTable("Airline", "core");

            entity.Property(e => e.AirlineCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Website).HasMaxLength(200);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportCode).HasName("PK__Airport__4B6773522057FD02");

            entity.ToTable("Airport", "core");

            entity.Property(e => e.AirportCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F2398F388C87C");

            entity.ToTable("AuditLog", "admin");

            entity.Property(e => e.Action).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Entity).HasMaxLength(100);

            entity.HasOne(d => d.Account).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__AuditLog__Accoun__22751F6C");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.BannerId).HasName("PK__Banner__32E86AD106B8CC88");

            entity.ToTable("Banner", "core");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LinkUrl).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Banners)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Banner_Account");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Banners)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("FK_Banner_Promotion");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AEDBE6CB70B");

            entity.ToTable("Booking", "sales");

            entity.HasIndex(e => e.BookingCode, "UQ__Booking__C6E56BD586AC063A").IsUnique();

            entity.Property(e => e.BookingCode).HasMaxLength(12);
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Account).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Booking__Account__01142BA1");
        });

        modelBuilder.Entity<BookingFlight>(entity =>
        {
            entity.HasKey(e => e.BookingFlightId).HasName("PK__BookingF__E29A26141B371862");

            entity.ToTable("BookingFlight", "sales");

            entity.Property(e => e.Fare).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PassengerName).HasMaxLength(150);
            entity.Property(e => e.PassengerType).HasMaxLength(10);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingFlights)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingFl__Booki__05D8E0BE");

            entity.HasOne(d => d.FlightInstance).WithMany(p => p.BookingFlights)
                .HasForeignKey(d => d.FlightInstanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BookingFl__Fligh__06CD04F7");

            entity.HasOne(d => d.Seat).WithMany(p => p.BookingFlights)
                .HasForeignKey(d => d.SeatId)
                .HasConstraintName("FK__BookingFl__SeatI__07C12930");
        });

        modelBuilder.Entity<BookingService>(entity =>
        {
            entity.HasKey(e => e.BookingServiceId).HasName("PK__BookingS__43F55CB195B02697");

            entity.ToTable("BookingService", "sales");

            entity.HasIndex(e => new { e.BookingId, e.ServiceId }, "UQ_Booking_Service").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK_BookingService_Booking");

            entity.HasOne(d => d.Service).WithMany(p => p.BookingServices)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_BookingService_Service");
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("PK__EmailVer__306D490795B440C3");

            entity.ToTable("EmailVerification", "auth");

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);

            entity.HasOne(d => d.Account).WithMany(p => p.EmailVerifications)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmailVeri__Accou__44FF419A");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__Flight__8A9E14EE302A588D");

            entity.ToTable("Flight", "core");

            entity.HasIndex(e => e.FlightNumber, "UQ__Flight__2EAE6F50AC25B70E").IsUnique();

            entity.Property(e => e.AirlineCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FlightNumber).HasMaxLength(20);

            entity.HasOne(d => d.AirlineCodeNavigation).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AirlineCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__AirlineC__60A75C0F");
        });

        modelBuilder.Entity<FlightInstance>(entity =>
        {
            entity.HasKey(e => e.FlightInstanceId).HasName("PK__FlightIn__FD0BBB312CC06338");

            entity.ToTable("FlightInstance", "core");

            entity.Property(e => e.ArrivalAirport)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DepartureAirport)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");

            entity.HasOne(d => d.ArrivalAirportNavigation).WithMany(p => p.FlightInstanceArrivalAirportNavigations)
                .HasForeignKey(d => d.ArrivalAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightIns__Arriv__6754599E");

            entity.HasOne(d => d.DepartureAirportNavigation).WithMany(p => p.FlightInstanceDepartureAirportNavigations)
                .HasForeignKey(d => d.DepartureAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightIns__Depar__66603565");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightInstances)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightIns__Fligh__656C112C");
        });

        modelBuilder.Entity<LoyaltyTransaction>(entity =>
        {
            entity.HasKey(e => e.LoyaltyTransactionId).HasName("PK__LoyaltyT__5CFDF9F44CBFC0A4");

            entity.ToTable("LoyaltyTransaction", "sales");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Reason).HasMaxLength(200);

            entity.HasOne(d => d.Account).WithMany(p => p.LoyaltyTransactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoyaltyTr__Accou__14270015");
        });

        modelBuilder.Entity<MembershipTier>(entity =>
        {
            entity.HasKey(e => e.MembershipTierId).HasName("PK__Membersh__B741E030FF51BBE7");

            entity.ToTable("MembershipTier", "admin");

            entity.Property(e => e.Benefits).HasMaxLength(500);
            entity.Property(e => e.DiscountPercent)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MinPoints).HasDefaultValue(0);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12BB754D54");

            entity.ToTable("Notification", "admin");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Message).HasMaxLength(2000);
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.Account).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Notificat__Accou__1EA48E88");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.OtpId).HasName("PK__OTP__3143C4A386DCA752");

            entity.ToTable("OTP", "auth");

            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Destination).HasMaxLength(150);
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.Purpose).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Otps)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__OTP__AccountId__49C3F6B7");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A3895D9FD2C");

            entity.ToTable("Payment", "sales");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaidAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__Booking__10566F31");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42FCFD8AAF92A");

            entity.ToTable("Promotion", "core");

            entity.HasIndex(e => e.Code, "UQ__Promotio__A25C5AA7781EE520").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A335E34EE");

            entity.ToTable("Role", "auth");

            entity.HasIndex(e => e.RoleName, "UQ__Role__8A2B61603B445CB4").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PK__Seat__311713F30F527461");

            entity.ToTable("Seat", "core");

            entity.HasIndex(e => new { e.FlightInstanceId, e.SeatNumber }, "UQ_Seat_Flight").IsUnique();

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.SeatNumber).HasMaxLength(10);

            entity.HasOne(d => d.FlightInstance).WithMany(p => p.Seats)
                .HasForeignKey(d => d.FlightInstanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__FlightInst__73BA3083");

            entity.HasOne(d => d.SeatClass).WithMany(p => p.Seats)
                .HasForeignKey(d => d.SeatClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Seat__SeatClassI__74AE54BC");
        });

        modelBuilder.Entity<SeatClass>(entity =>
        {
            entity.HasKey(e => e.SeatClassId).HasName("PK__SeatClas__41D153A8F043BFBC");

            entity.ToTable("SeatClass", "core");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PriceMultiplier)
                .HasDefaultValue(1.0m)
                .HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Service__C51BB00A071CC9E4");

            entity.ToTable("Service", "sales");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__SupportT__712CC607927E6ABA");

            entity.ToTable("SupportTicket", "admin");

            entity.Property(e => e.Content).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Open");
            entity.Property(e => e.Subject).HasMaxLength(300);

            entity.HasOne(d => d.Account).WithMany(p => p.SupportTickets)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__SupportTi__Accou__19DFD96B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
