using AutoMapper;
using VacationRental.Api.Models;

namespace VacationRental.Api.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Rental, RentalViewModel>();
            CreateMap<RentalViewModel, Rental>();
            CreateMap<RentalBindingModel, Rental>();
        }
    }
}