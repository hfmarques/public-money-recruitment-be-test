using System;
using AutoMapper;
using Moq;
using VacationRental.Api.Configurations;
using VacationRental.Api.Models.Entities;
using VacationRental.Api.Models.ViewModels;
using VacationRental.Api.Repositories.Interfaces;
using VacationRental.Api.Services;
using VacationRental.Api.Services.Interfaces;
using Xunit;

namespace VacationRental.Api.Tests.Services
{
    [Collection("Unit")]
    public class RentalServiceTests
    {
        private readonly Mock<IRentalRepository> _rentalRepository;
        private readonly IRentalService _rentalService;

        public RentalServiceTests()
        {
            var profile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(profile));
            IMapper mapper = new Mapper(configuration);

            _rentalRepository = new Mock<IRentalRepository>();
            _rentalService = new RentalService(mapper, _rentalRepository.Object);
        }

        [Fact]
        public void GetById_IdIsZero_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _rentalService.GetById(0));
        }

        [Fact]
        public void GetById_ValidId_ReturnRentalObject()
        {
            const int id = 1;
            const int units = 25;
            _rentalRepository.Setup(x => x.GetById(id))
                .Returns(new Rental
                {
                    Id = id,
                    Units = units
                });

            var result = _rentalService.GetById(id);

            Assert.Equal(result.Id, id);
            Assert.Equal(result.Units, units);
        }

        [Fact]
        public void Add_RentalIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _rentalService.Add(null));
        }

        [Fact]
        public void Add_RentalIsValid_AddTheRental()
        {
            const int units = 25;
            var rental = new RentalBindingModel
            {
                Units = units
            };

            var result = _rentalService.Add(rental);

            _rentalRepository.Verify(x => x.Add(It.IsAny<Rental>()), Times.Once);
            Assert.Equal(result.Units, units);
        }

        [Fact]
        public void Update_RentalIsNull_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _rentalService.Update(0, null));
        }

        [Fact]
        public void Update_RentalIdIsZero_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _rentalService.Update(0, new RentalBindingModel()));
        }

        [Fact]
        public void Update_RentalDoesNotExists_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _rentalService.Update(1, new RentalBindingModel()));
        }

        [Fact]
        public void Update_RentalIsValid_UpdateTheRental()
        {
            var rental = new RentalBindingModel
            {
                Units = 25,
            };

            _rentalRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns(new Rental {Id = 1});

            _rentalService.Update(1, rental);

            _rentalRepository.Verify(x => x.Update(It.IsAny<Rental>()), Times.Once);
        }
    }
}