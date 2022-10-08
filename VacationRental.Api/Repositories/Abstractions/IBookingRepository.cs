using System.Collections.Generic;
using VacationRental.Api.Models.Entities;

namespace VacationRental.Api.Repositories.Abstractions
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetAll();
    }
}