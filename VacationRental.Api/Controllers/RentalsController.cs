using System;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IBookingService _bookingService;

        public RentalsController(IRentalService rentalService, IBookingService bookingService)
        {
            _rentalService = rentalService;
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public IActionResult Get(int rentalId)
        {
            try
            {
                var rental = _rentalService.GetById(rentalId);

                if (rental is null)
                    return NotFound("Rental not found");

                return Ok(rental);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post(RentalBindingModel model)
        {
            try
            {
                var rentalViewModel = _rentalService.Add(model);

                return Created("", rentalViewModel);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public IActionResult Put(int rentalId, [FromBody] RentalBindingModel model)
        {
            try
            {
                if (_bookingService.PreparationTimeChangeIsAllowed(model.PreparationTimeInDays))
                    _rentalService.Update(rentalId, model);
                else
                    return BadRequest(
                        "There is a overlapping between existing bookings and/or their preparation times");

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}