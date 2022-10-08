using System.Collections.Generic;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;

namespace VacationRental.Api.Services
{
    public interface IBookingService
    {
        BookingViewModel GetById(int id);
        bool VerifyBookingAvailability(Booking booking);
        BookingViewModel Add(BookingBindingModel bookingBindingModel);
        List<BookingViewModel> GetAll();
    }
}