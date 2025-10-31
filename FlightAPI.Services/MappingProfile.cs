// FlightAPI.Services/MappingProfile.cs

using AutoMapper;
using FlightAPI.Data.Entities;
using FlightAPI.Data.Models;
using FlightAPI.Models;

namespace FlightAPI.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Định nghĩa quy tắc ánh xạ từ Entity sang DTO
            CreateMap<Airport, AirportReadDto>();

            // Định nghĩa quy tắc ánh xạ từ DTO sang Entity (khi tạo mới)
            CreateMap<AirportCreateDto, Airport>();

            // Từ DTO Request sang Entity
            CreateMap<FlightManagerServiceCreateRequest, Flight>();
            CreateMap<FlightManagerServiceUpdateRequest, Flight>();

            // Từ Entity sang DTO Hiển thị
            CreateMap<Flight, FlightManagerServiceDto>();

            CreateMap <AirlineCreateDto, Airline>();
            CreateMap<Airline, AirlineCreateDto>();


            // --- 2. SỬA LỖI KHI ĐỌC (READ/GET) ---
            // Ánh xạ các trường bị rỗng/sai trong JSON Response
            CreateMap<FlightInstanceCreateDto, FlightInstance>();
            CreateMap<FlightInstance, FlightInstanceReadDto>()
                // SỬA CÁC TRƯỜNG BỊ RỖNG/SAI
                .ForMember(dest => dest.FlightNumber, // Tên trong DTO
                           opt => opt.MapFrom(src => src.Flight.FlightNumber)) // Lấy từ Entity liên quan

                .ForMember(dest => dest.DepartureAirportCode, // Tên trong DTO
                           opt => opt.MapFrom(src => src.DepartureAirport)) // Lấy từ Entity

                .ForMember(dest => dest.ArrivalAirportCode, // Tên trong DTO
                           opt => opt.MapFrom(src => src.ArrivalAirport)) // Lấy từ Entity

                .ForMember(dest => dest.BasePrice, // Tên trong DTO
                           opt => opt.MapFrom(src => src.BasePrice)) // Lấy từ Entity (khác chữ hoa/thường)

                // CÁC TRƯỜNG ĐÃ ĐÚNG (từ dữ liệu liên quan)
                .ForMember(dest => dest.AirlineName,
                           opt => opt.MapFrom(src => src.Flight.AirlineCodeNavigation.Name))
                .ForMember(dest => dest.DepartureCity,
                           opt => opt.MapFrom(src => src.DepartureAirportNavigation.City))
                .ForMember(dest => dest.ArrivalCity,
                           opt => opt.MapFrom(src => src.ArrivalAirportNavigation.City))

                // Ánh xạ Khóa chính (nếu tên khác nhau)
                .ForMember(dest => dest.FlightInstanceId,
                           opt => opt.MapFrom(src => src.FlightInstanceId));
        }
    }
}