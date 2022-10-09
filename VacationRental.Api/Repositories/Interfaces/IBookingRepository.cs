using System.Collections.Generic;
using VacationRental.Api.Models.Entities;

namespace VacationRental.Api.Repositories.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetAll();
    }
}