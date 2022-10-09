﻿using System.Collections.Generic;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;

namespace VacationRental.Api.Services
{
    public interface IBookingService
    {
        BookingViewModel GetById(int id);
        bool VerifyBookingAvailability(Booking booking, List<Booking> existingBookings);
        BookingViewModel Add(BookingBindingModel bookingBindingModel);
        void Update(BookingViewModel bookingViewModel);
        List<BookingViewModel> GetAll();
    }
}