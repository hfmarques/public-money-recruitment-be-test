using System;
using VacationRental.Api.Models.ViewModels;

namespace VacationRental.Api.Services.Interfaces
{
    public interface ICalendarService
    {
        CalendarViewModel Get(int rentalId, DateTime start, int nights);
        CalendarViewModel GetWithUnitAndPreparationTimes(int rentalId, DateTime start, int nights);
    }
}