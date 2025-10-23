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

            CreateMap<FlightInstanceCreateDto, FlightInstance>();

            // Từ DTO Request sang Entity
            CreateMap<FlightManagerServiceCreateRequest, Flight>();
            CreateMap<FlightManagerServiceUpdateRequest, Flight>();

            // Từ Entity sang DTO Hiển thị
            CreateMap<Flight, FlightManagerServiceDto>();

            // Mapping phức tạp: Ánh xạ nhiều trường từ các Entity liên quan
            CreateMap<FlightInstance, FlightInstanceReadDto>()
                .ForMember(dest => dest.AirlineName,
                           opt => opt.MapFrom(src => src.Flight.AirlineCodeNavigation.Name))
                .ForMember(dest => dest.DepartureCity,
                           opt => opt.MapFrom(src => src.DepartureAirportNavigation.City))
                .ForMember(dest => dest.ArrivalCity,
                           opt => opt.MapFrom(src => src.ArrivalAirportNavigation.City));
        }
    }
}