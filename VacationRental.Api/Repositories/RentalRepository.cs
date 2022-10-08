using System.Collections.Generic;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Repositories.Abstractions;

namespace VacationRental.Api.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly IDictionary<int, Rental> _rentals;

        public RentalRepository(IDictionary<int, Rental> rentals)
        {
            _rentals = rentals;
        }

        public Rental GetById(int id)
        {
            _rentals.TryGetValue(id, out var rental);
            return rental;
        }

        public int Add(Rental rental)
        {
            var id = _rentals.Keys.Count + 1;
            rental.Id = id;

            _rentals.Add(id, new Rental
            {
                Id = id,
                Units = rental.Units
            });

            return id;
        }

        public void Update(Rental rental)
        {
            _rentals[rental.Id] = rental;
        }
    }
}