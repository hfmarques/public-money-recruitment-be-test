using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Repositories.Interfaces;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IRentalService _rentalService;

        public BookingService(IMapper mapper,
            IBookingRepository bookingRepository,
            IRentalService rentalService)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _rentalService = rentalService;
        }

        public BookingViewModel GetById(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id cannot be zero", nameof(id));

            var booking = _bookingRepository.GetById(id);

            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);

            return bookingViewModel;
        }

        public bool VerifyBookingAvailability(Booking model, List<Booking> existingBookings)
        {
            for (var i = 0; i < model.Nights; i++)
            {
                var count = 0;
                foreach (var booking in existingBookings)
                {
                    if (booking.RentalId == model.RentalId
                        && (booking.Start.Date <= model.Start.Date &&
                            booking.Start.Date.AddDays(booking.Nights) > model.Start.Date)
                        || (booking.Start.Date < model.Start.AddDays(model.Nights) &&
                            booking.Start.Date.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                        || (booking.Start.Date > model.Start &&
                            booking.Start.Date.AddDays(booking.Nights) < model.Start.AddDays(model.Nights)))
                    {
                        count++;
                    }
                }

                if (count >= _rentalService.GetById(model.RentalId).Units)
                    return false;
            }

            return true;
        }

        public BookingViewModel Add(BookingBindingModel bookingBindingModel)
        {
            if (bookingBindingModel == null)
            {
                throw new ArgumentNullException(nameof(bookingBindingModel), "Booking cannot be null");
            }

            if (bookingBindingModel.Nights <= 0)
                throw new ArgumentException("Nigts must be positive", nameof(bookingBindingModel.Nights));
            if (_rentalService.GetById(bookingBindingModel.RentalId) is null)
                throw new ArgumentException("Rental not found", nameof(bookingBindingModel.RentalId));

            var booking = _mapper.Map<Booking>(bookingBindingModel);
            var validBooking = VerifyBookingAvailability(
                booking,
                _bookingRepository.GetAll().ToList());
            if (!validBooking)
            {
                throw new Exception("Not available");
            }

            _bookingRepository.Add(booking);

            var bookingViewModel = _mapper.Map<BookingViewModel>(booking);
            return bookingViewModel;
        }

        public void Update(BookingViewModel bookingViewModel)
        {
            if (bookingViewModel == null)
            {
                throw new ArgumentNullException(nameof(bookingViewModel), "Booking cannot be null");
            }

            if (bookingViewModel.Id == 0)
            {
                throw new ArgumentException("Id cannot be zero", nameof(bookingViewModel.Id));
            }
            
            if (GetById(bookingViewModel.Id) is null)
            {
                throw new ArgumentException("Booking does not exists");
            }
            
            var booking = _mapper.Map<Booking>(bookingViewModel);

            var existingBookings = _bookingRepository
                .GetAll()
                .Where(x => x.Id != booking.Id).ToList();
            var validBooking = VerifyBookingAvailability(
                booking,
                existingBookings
                );
            if (!validBooking)
            {
                throw new Exception("Not available");
            }

            _bookingRepository.Update(booking);
        }

        public List<BookingViewModel> GetAll()
        {
            var bookings = _bookingRepository.GetAll().ToList();
            var bookingsViewModel = _mapper.Map<List<BookingViewModel>>(bookings);
            return bookingsViewModel;
        }
    }
}