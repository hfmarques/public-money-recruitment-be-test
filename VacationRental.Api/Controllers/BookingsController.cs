using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("{bookingId:int}")]
        public IActionResult Get(int bookingId)
        {
            try
            {
                var booking = _bookingService.GetById(bookingId);

                if (booking is null)
                    return NotFound("Booking not found");

                return Ok(booking);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post(BookingBindingModel model)
        {
            try
            {
                var bookingViewModel = _bookingService.Add(model);

                return Created("", bookingViewModel);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}