using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Repositories.Abstractions;

namespace VacationRental.Api.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDictionary<int, Booking> _bookings;

        public BookingRepository(IDictionary<int, Booking> bookings)
        {
            _bookings = bookings;
        }

        public IEnumerable<Booking> GetAll()
        {
            return _bookings.Select(x => new Booking
            {
                Id = x.Value.Id,
                Nights = x.Value.Nights,
                RentalId = x.Value.RentalId,
                Start = x.Value.Start.Date
            });
        }

        public Booking GetById(int id)
        {
            _bookings.TryGetValue(id, out var booking);
            return booking;
        }

        public int Add(Booking booking)
        {
            var id = _bookings.Keys.Count + 1;
            booking.Id = id;

            _bookings.Add(id, new Booking
            {
                Id = id,
                Nights = booking.Nights,
                RentalId = booking.RentalId,
                Start = booking.Start.Date
            });

            return id;
        }

        public void Update(Booking booking)
        {
            _bookings[booking.Id] = booking;
        }
    }
}