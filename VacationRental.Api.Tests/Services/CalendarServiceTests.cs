using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;
using Xunit;

namespace VacationRental.Api.Tests.Services
{
    [Collection("Unit")]
    public class CalendarServiceTests
    {
        private readonly Mock<IBookingService> _bookingService;
        private readonly Mock<IRentalService> _rentalService;
        private readonly ICalendarService _calendarService;

        public CalendarServiceTests()
        {
            _bookingService = new Mock<IBookingService>();
            _rentalService = new Mock<IRentalService>();
            _calendarService = new CalendarService(_rentalService.Object, _bookingService.Object);

            _rentalService.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(new RentalViewModel(){Units = 2, PreparationTimeInDays = 1});
            _bookingService.Setup(x => x.GetAll())
                .Returns(new List<BookingViewModel>
                {
                    new BookingViewModel
                    {
                        Id = 1,
                        Nights = 2,
                        Start = DateTime.Now.AddDays(3),
                        RentalId = 1,
                    },
                    new BookingViewModel
                    {
                        Id = 2,
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3),
                        RentalId = 1,
                    },
                });
        }

        [Fact]
        public void Get_NightsIsNegative_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _calendarService.Get(1, DateTime.Now, -1));
        }

        [Fact]
        public void Get_RentalDoesNotExists_ThrowArgumentException()
        {
            _rentalService.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((RentalViewModel) null);
            Assert.Throws<ArgumentException>(() => _calendarService.Get(1, DateTime.Now, 4));
        }

        [Fact]
        public void Get_NightIsZero_DatesIsEmpty()
        {
            var result = _calendarService.Get(1, DateTime.Now, 0);

            Assert.Empty(result.Dates);
        }
        
        [Fact]
        public void Get_NoBookingWithRentalIdTwo_DateBookingIsEmpty()
        {
            var result = _calendarService.Get(2, DateTime.Now, 1);

            Assert.Empty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void Get_NoBookingWithStartDateGreaterThanDate_DateBookingIsEmpty()
        {
            var result = _calendarService.Get(1, DateTime.Now.AddDays(10), 1);

            Assert.Empty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void Get_NoBookingWithEndDateGreaterThanDate_DateBookingIsEmpty()
        {
            var result = _calendarService.Get(1, DateTime.Now.AddDays(1), 1);

            Assert.Empty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void Get_HasValidBookings_ReturnBookings()
        {
            var result = _calendarService.Get(1, DateTime.Now.AddDays(4), 1);

            Assert.NotEmpty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_BookingFound_DateBookingIsNotEmpty()
        {
            var result = _calendarService.GetWithUnitAndPreparationTimes(1, DateTime.Now.AddDays(4), 1);

            Assert.NotEmpty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_NightsIsNegative_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _calendarService.GetWithUnitAndPreparationTimes(1, DateTime.Now, -1));
        }

        [Fact]
        public void GetWithUnitAndPreparationTimes_RentalDoesNotExists_ThrowArgumentException()
        {
            _rentalService.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((RentalViewModel) null);
            Assert.Throws<ArgumentException>(() => _calendarService.GetWithUnitAndPreparationTimes(1, DateTime.Now, 4));
        }

        [Fact]
        public void GetWithUnitAndPreparationTimes_NightIsZero_DatesIsEmpty()
        {
            var result = _calendarService.GetWithUnitAndPreparationTimes(1, DateTime.Now, 0);

            Assert.Empty(result.Dates);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_NoBookingWithRentalIdTwo_DateBookingIsEmpty()
        {
            var result = _calendarService.GetWithUnitAndPreparationTimes(2, DateTime.Now, 1);

            Assert.Empty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_NoBookingWithStartDateGreaterThanDate_DateBookingIsEmpty()
        {
            var result = _calendarService.GetWithUnitAndPreparationTimes(1, DateTime.Now.AddDays(10), 1);

            Assert.Empty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_NoBookingWithEndDateGreaterThanDate_DateBookingIsEmpty()
        {
            var result = _calendarService.GetWithUnitAndPreparationTimes(1, DateTime.Now.AddDays(1), 1);

            Assert.Empty(result.Dates.First().Bookings);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_OneBooking_TwoDaysWithBookAndOneWithPreparationTime()
        {
            _bookingService.Setup(x => x.GetAll())
                .Returns(new List<BookingViewModel>
                {
                    new BookingViewModel
                    {
                        Id = 1,
                        Nights = 2,
                        Start = new DateTime(2020, 01, 01),
                        RentalId = 1,
                    },
                });
            var result = _calendarService.GetWithUnitAndPreparationTimes(1, new DateTime(2020, 01, 01), 3);

            Assert.Equal(1, result.Dates[0].Bookings.First().Unit);
            Assert.Equal(1, result.Dates[1].Bookings.First().Unit);
            Assert.Equal(1, result.Dates[2].PreparationTimes.First().Unit);
            
            Assert.Empty(result.Dates[0].PreparationTimes);
            Assert.Empty(result.Dates[1].PreparationTimes);
            Assert.Empty(result.Dates[2].Bookings);
        }
        
        [Fact]
        public void GetWithUnitAndPreparationTimes_MultipleBookings_ReturnTheRightCalendar()
        {
            _bookingService.Setup(x => x.GetAll())
                .Returns(new List<BookingViewModel>
                {
                    new BookingViewModel
                    {
                        Id = 1,
                        Nights = 2,
                        Start = new DateTime(2020, 01, 01),
                        RentalId = 1,
                    },
                    new BookingViewModel
                    {
                        Id = 2,
                        Nights = 3,
                        Start = new DateTime(2020, 01, 01),
                        RentalId = 1,
                    },
                    new BookingViewModel
                    {
                        Id = 3,
                        Nights = 1,
                        Start = new DateTime(2020, 01, 04),
                        RentalId = 1,
                    },
                });
            var result = _calendarService.GetWithUnitAndPreparationTimes(1, new DateTime(2020, 01, 01), 5);

            Assert.Equal(1, result.Dates[0].Bookings[0].Unit);
            Assert.Equal(2, result.Dates[0].Bookings[1].Unit);
            Assert.Equal(1, result.Dates[1].Bookings[0].Unit);
            Assert.Equal(2, result.Dates[1].Bookings[1].Unit);
            Assert.Equal(1, result.Dates[2].PreparationTimes[0].Unit);
            Assert.Equal(2, result.Dates[2].Bookings[0].Unit);
            Assert.Equal(2, result.Dates[3].PreparationTimes[0].Unit);
            Assert.Equal(1, result.Dates[3].Bookings[0].Unit);
            Assert.Equal(1, result.Dates[4].PreparationTimes[0].Unit);
        }
    }
}