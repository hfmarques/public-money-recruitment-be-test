using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Services
{
    public class CalendarService : ICalendarService
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;

        public CalendarService(IRentalService rentalService, IBookingService bookingService)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;
        }

        public CalendarViewModel Get(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ArgumentException("Nights must be positive", nameof(nights));
            if (_rentalService.GetById(rentalId) is null)
                throw new ArgumentException("Rental not found", nameof(rentalId));

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>()
                };

                foreach (var booking in _bookingService.GetAll().Where(booking =>
                             booking.RentalId == rentalId &&
                             booking.Start <= date.Date &&
                             booking.Start.AddDays(booking.Nights) > date.Date))
                {
                    date.Bookings.Add(new CalendarBookingViewModel {Id = booking.Id});
                }

                result.Dates.Add(date);
            }

            return result;
        }
    }
}