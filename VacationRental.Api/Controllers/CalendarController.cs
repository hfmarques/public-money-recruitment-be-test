using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet]
        public IActionResult Get(int rentalId, DateTime start, int nights)
        {
            try
            {
                var calendar = _calendarService.GetWithUnitAndPreparationTimes(rentalId, start, nights);

                return Ok(calendar);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}