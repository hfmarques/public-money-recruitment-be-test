using VacationRental.Api.Models;

namespace VacationRental.Api.Services
{
    public interface IRentalService
    {
        RentalViewModel GetById(int id);
        RentalViewModel Add(RentalBindingModel rentalBindingModel);
        void Update(RentalViewModel rentalViewModel);
    }
}