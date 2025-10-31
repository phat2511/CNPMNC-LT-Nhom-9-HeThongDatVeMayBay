using FlightAPI.Services.Dtos.Booking;
using FlightAPI.Services.Dtos.Payment;

namespace FlightAPI.Services
{
    public interface IBookedService
    {
        Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto dto, int accountId);

        Task SelectSeatAsync(SelectSeatRequestDto dto, int accountId);

        Task<PaymentResponseDto> ProcessPaymentAsync(int bookingId, PaymentRequestDto dto, int accountId);
    }
}