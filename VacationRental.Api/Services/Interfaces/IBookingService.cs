using System.Collections.Generic;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;

namespace VacationRental.Api.Services.Interfaces
{
    public interface IBookingService
    {
        BookingViewModel GetById(int id);
        bool VerifyBookingAvailability(Booking booking, List<Booking> existingBookings);

        bool VerifyBookingAvailabilityWithPreparationTime(Booking booking, List<Booking> existingBookings,
            int preparationTime);

        BookingViewModel Add(BookingBindingModel bookingBindingModel);
        void Update(BookingViewModel bookingViewModel);
        List<BookingViewModel> GetAll();
        bool PreparationTimeChangeIsAllowed(int newPreparationTime);
    }
}