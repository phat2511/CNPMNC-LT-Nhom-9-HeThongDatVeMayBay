namespace FlightAPI.Services.Dtos.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string PaymentStatus { get; set; } = null!; // "Success", "Failed"
        public string BookingStatus { get; set; } = null!; // "Confirmed"
        public decimal AmountPaid { get; set; }
        public DateTime PaidAt { get; set; }
    }
}