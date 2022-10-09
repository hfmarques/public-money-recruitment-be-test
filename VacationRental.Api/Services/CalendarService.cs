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
        
        public CalendarViewModel GetWithUnitAndPreparationTimes(int rentalId, DateTime start, int nights)
        {
            if (nights < 0)
                throw new ArgumentException("Nights must be positive", nameof(nights));

            var rental = _rentalService.GetById(rentalId);
            if (rental is null)
                throw new ArgumentException("Rental not found", nameof(rentalId));

            var result = new CalendarViewModel
            {
                RentalId = rentalId,
                Dates = new List<CalendarDateViewModel>()
            };

            var unitControl = new Dictionary<int, int>();

            for (var i = 0; i < nights; i++)
            {
                var date = new CalendarDateViewModel
                {
                    Date = start.Date.AddDays(i),
                    Bookings = new List<CalendarBookingViewModel>(),
                    PreparationTimes = new List<CalendarPreparationTimeViewModel>()
                };

                foreach (var booking in _bookingService.GetAll().Where(booking =>
                             booking.RentalId == rentalId &&
                             booking.Start <= date.Date &&
                             booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays) > date.Date))
                {
                    var bookingUnit = result.Dates.FirstOrDefault(x =>
                            x.Bookings.Exists(b => b.Id == booking.Id))?
                        .Bookings.First(x => x.Id == booking.Id).Unit;

                    if (booking.Start.AddDays(booking.Nights) > date.Date)
                    {
                        HandleBookingsCalendar(rentalId, bookingUnit, unitControl, date, booking);
                    }
                    else
                    {
                        HandlePreparationTimesCalendar(rentalId, date, (int) bookingUnit, unitControl);
                    }
                }

                result.Dates.Add(date);
            }

            return result;
        }

        private static void HandlePreparationTimesCalendar(int rentalId, CalendarDateViewModel date, int bookingUnit,
            IDictionary<int, int> unitControl)
        {
            date.PreparationTimes.Add(
                new CalendarPreparationTimeViewModel
                {
                    Unit = bookingUnit
                }
            );

            if (unitControl[rentalId] == 1)
            {
                unitControl.Remove(rentalId);
            }
            else
            {
                unitControl[rentalId]--;
            }
        }

        private static void HandleBookingsCalendar(int rentalId, int? bookingUnit, IDictionary<int, int> unitControl,
            CalendarDateViewModel date,
            BookingViewModel booking)
        {
            if (bookingUnit is null)
            {
                if (unitControl.ContainsKey(rentalId))
                {
                    unitControl[rentalId]++;
                    bookingUnit = unitControl[rentalId];
                }
                else
                {
                    unitControl.Add(rentalId, 1);
                    bookingUnit = 1;
                }
            }

            date.Bookings.Add(new CalendarBookingViewModel {Id = booking.Id, Unit = (int) bookingUnit});
        }
    }
}