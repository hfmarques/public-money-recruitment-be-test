using AutoMapper;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;

namespace VacationRental.Api.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Rental, RentalViewModel>();
            CreateMap<RentalViewModel, Rental>();
            CreateMap<RentalBindingModel, Rental>();

            CreateMap<Booking, BookingViewModel>();
            CreateMap<BookingViewModel, Booking>();
            CreateMap<BookingBindingModel, Booking>();
        }
    }
}