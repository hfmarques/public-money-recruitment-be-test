using System;

namespace VacationRental.Api.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int RentalId { get; set; }
        public DateTime Start { get; set; }
        public int Nights { get; set; }
    }
}