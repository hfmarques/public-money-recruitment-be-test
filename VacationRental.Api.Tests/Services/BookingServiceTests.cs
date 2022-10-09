using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using VacationRental.Api.Configurations;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Repositories.Interfaces;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;
using Xunit;

namespace VacationRental.Api.Tests.Services
{
    [Collection("Unit")]
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepository;
        private readonly IBookingService _bookingService;

        public BookingServiceTests()
        {
            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(profile));
            IMapper mapper = new Mapper(configuration);

            _bookingRepository = new Mock<IBookingRepository>();
            var rentalService = new Mock<IRentalService>();
            
            _bookingService = new BookingService(mapper, _bookingRepository.Object, rentalService.Object);

            rentalService.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(new RentalViewModel {Id = 1, Units = 2, PreparationTimeInDays = 1});
        }

        [Fact]
        public void GetById_IdIsZero_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _bookingService.GetById(0));
        }

        [Fact]
        public void GetById_ValidId_ReturnBookingObject()
        {
            const int id = 1;
            const int nights = 3;
            _bookingRepository.Setup(x => x.GetById(id))
                .Returns(new Booking
                {
                    Id = id,
                    Nights = nights,
                    Start = DateTime.Now,
                    RentalId = 1
                });

            var result = _bookingService.GetById(id);

            Assert.Equal(result.Id, id);
            Assert.Equal(result.Nights, nights);
        }

        [Theory]//existing booking days start at day 3 and finish at day 6
        [InlineData(2, 1)]//starts on day 2 and finish at day 3
        [InlineData(2, 2)]//starts on day 2 and finish at day 4
        [InlineData(3, 3)]//starts on day 3 and finish at day 6
        [InlineData(4, 1)]//starts on day 4 and finish at day 5
        [InlineData(5, 2)]//starts on day 5 and finish at day 7
        [InlineData(2, 5)]//starts on day 2 and finish at day 7
        public void VerifyBookingAvailability_BookingOverlapExist_ReturnsFalse(int startDay, int nights)
        {
            var existingBookings =
                new List<Booking>
                {
                    new Booking
                    {
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3).Date,
                        RentalId = 1,
                    },
                    new Booking
                    {
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3).Date,
                        RentalId = 1,
                    },
                };

            var booking = new Booking
            {
                Start = DateTime.Now.AddDays(startDay),
                Nights = nights,
                RentalId = 1,
            };

            var result = _bookingService.VerifyBookingAvailability(booking, existingBookings);

            Assert.True(!result);
        }

        [Fact]
        public void VerifyBookingAvailability_NoBookingOverlapExist_ReturnsTrue()
        {
            var existingBookings =
                new List<Booking>
                {
                    new Booking
                    {
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3).Date,
                        RentalId = 1,
                    },
                };

            var booking = new Booking
            {
                Start = DateTime.Now.AddDays(2),
                Nights = 2,
                RentalId = 1,
            };

            var result = _bookingService.VerifyBookingAvailability(booking, existingBookings);

            Assert.True(result);
        }
        
        [Theory]//existing booking days start at day 3, has 6 nights and 1 preparation day, so it finishes at day 7
        [InlineData(2, 1)]//starts on day 2 and finish at day 3+1=4
        [InlineData(2, 2)]//starts on day 2 and finish at day 4+1=5
        [InlineData(3, 3)]//starts on day 3 and finish at day 6+1=7
        [InlineData(4, 1)]//starts on day 4 and finish at day 5+1=6
        [InlineData(5, 2)]//starts on day 5 and finish at day 7+1=8
        [InlineData(2, 5)]//starts on day 2 and finish at day 7+1=8
        [InlineData(1, 1)]//starts on day 1 and finish at day 2+1=3
        [InlineData(6, 1)]//starts on day 6 and finish at day 6+1=8
        public void VerifyBookingAvailabilityWithPreparationTime_BookingOverlapExist_ReturnsFalse(int startDay, int nights)
        {
            var existingBookings =
                new List<Booking>
                {
                    new Booking
                    {
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3).Date,
                        RentalId = 1,
                    },
                    new Booking
                    {
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3).Date,
                        RentalId = 1,
                    },
                };
            
            

            var booking = new Booking
            {
                Start = DateTime.Now.AddDays(startDay),
                Nights = nights,
                RentalId = 1,
            };

            var result = _bookingService.VerifyBookingAvailabilityWithPreparationTime(booking, existingBookings);

            Assert.True(!result);
        }

        [Fact]
        public void VerifyBookingAvailabilityWithPreparationTime_NoBookingOverlapExist_ReturnsTrue()
        {
            var existingBookings =
                new List<Booking>
                {
                    new Booking
                    {
                        Nights = 3,
                        Start = DateTime.Now.AddDays(3).Date,
                        RentalId = 1,
                    },
                };

            var booking = new Booking
            {
                Start = DateTime.Now.AddDays(2),
                Nights = 2,
                RentalId = 1,
            };

            var result = _bookingService.VerifyBookingAvailabilityWithPreparationTime(booking, existingBookings);

            Assert.True(result);
        }

        [Fact]
        public void Add_BookingIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _bookingService.Add(null));
        }

        [Fact]
        public void Add_BookingIsValid_AddTheBooking()
        {
            const int nights = 3;
            var booking = new BookingBindingModel
            {
                Nights = nights,
                Start = DateTime.Now,
                RentalId = 1,
            };

            _bookingRepository.Setup(x => x.GetAll())
                .Returns(new List<Booking>());

            var result = _bookingService.Add(booking);

            _bookingRepository.Verify(x =>
                x.Add(It.IsAny<Booking>()), Times.Once);
            Assert.Equal(result.Nights, nights);
        }

        [Fact]
        public void Update_BookingIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _bookingService.Update(null));
        }

        [Fact]
        public void Update_BookingIdIsZero_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _bookingService.Update(new BookingViewModel {Id = 0}));
        }

        [Fact]
        public void Update_BookingDoesNotExists_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _bookingService.Update(new BookingViewModel {Id = 1}));
        }

        [Fact]
        public void Update_BookingIsValid_UpdateTheBooking()
        {
            var booking = new BookingViewModel
            {
                Id = 1,
                Nights = 1,
                Start = DateTime.Now,
                RentalId = 1,
            };

            _bookingRepository.Setup(x => x.GetAll())
                .Returns(new List<Booking>
                {
                    new Booking
                    {
                        Id = 1,
                        Nights = 1,
                        Start = DateTime.Now,
                        RentalId = 1,
                    },
                    new Booking
                    {
                        Id = 2,
                        Nights = 2,
                        Start = DateTime.Now,
                        RentalId = 1,
                    }
                });

            _bookingRepository.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Booking());

            _bookingService.Update(booking);

            _bookingRepository.Verify(x =>
                x.Update(It.IsAny<Booking>()), Times.Once);
        }
    }
}