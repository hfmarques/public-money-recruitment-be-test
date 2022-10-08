using System;
using AutoMapper;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Repositories;
using VacationRental.Api.Repositories.Abstractions;

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

        public void Update(RentalViewModel rentalViewModel)
        {
            if (rentalViewModel == null)
            {
                throw new ArgumentNullException(nameof(rentalViewModel), "Rental cannot be null");
            }

            if (rentalViewModel.Id == 0)
            {
                throw new ArgumentException("Id cannot be zero", nameof(rentalViewModel.Id));
            }

            var rental = _mapper.Map<Rental>(rentalViewModel);

            _rentalRepository.Update(rental);
        }
    }
}