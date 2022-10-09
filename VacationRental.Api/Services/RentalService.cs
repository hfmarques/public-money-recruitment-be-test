using System;
using AutoMapper;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Repositories.Interfaces;
using VacationRental.Api.Services.Interfaces;

namespace VacationRental.Api.Services
{
    public class RentalService : IRentalService
    {
        private readonly IMapper _mapper;
        private readonly IRentalRepository _rentalRepository;

        public RentalService(IMapper mapper, IRentalRepository rentalRepository)
        {
            _mapper = mapper;
            _rentalRepository = rentalRepository;
        }

        public RentalViewModel GetById(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id cannot be zero", nameof(id));

            var rental = _rentalRepository.GetById(id);

            var rentalViewModel = _mapper.Map<RentalViewModel>(rental);

            return rentalViewModel;
        }

        public RentalViewModel Add(RentalBindingModel rentalBindingModel)
        {
            if (rentalBindingModel == null)
            {
                throw new ArgumentNullException(nameof(rentalBindingModel), "Rental cannot be null");
            }

            var rental = _mapper.Map<Rental>(rentalBindingModel);

            _rentalRepository.Add(rental);

            var rentalViewModel = _mapper.Map<RentalViewModel>(rental);

            return rentalViewModel;
        }

        public void Update(int rentalId, RentalBindingModel rentalBindingModel)
        {
            if (rentalBindingModel == null)
            {
                throw new ArgumentNullException(nameof(rentalBindingModel), "Rental cannot be null");
            }

            if (rentalId == 0)
            {
                throw new ArgumentException("Id cannot be zero", nameof(rentalId));
            }

            if (GetById(rentalId) is null)
            {
                throw new ArgumentException("Rental does not exists");
            }

            var rental = _mapper.Map<Rental>(rentalBindingModel);

            rental.Id = rentalId;

            _rentalRepository.Update(rental);
        }
    }
}