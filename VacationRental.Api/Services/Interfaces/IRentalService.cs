using VacationRental.Api.Models.ViewModels;

namespace VacationRental.Api.Services.Interfaces
{
    public interface IRentalService
    {
        RentalViewModel GetById(int id);
        RentalViewModel Add(RentalBindingModel rentalBindingModel);
        void Update(RentalViewModel rentalViewModel);
    }
}