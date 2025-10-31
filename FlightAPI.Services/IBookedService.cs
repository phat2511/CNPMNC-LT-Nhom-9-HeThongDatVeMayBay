using FlightAPI.Services.Dtos.Booking;

namespace FlightAPI.Services
{
    public interface IBookedService
    {
        Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto dto, int accountId);
    }
}