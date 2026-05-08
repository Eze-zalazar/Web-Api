using Application.Interfaces;
using Application.UseCase.Eventos.Commands;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public class CreateEventHandler : ICreateEventHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public CreateEventHandler(IUnitOfWork unitOfWork, IEventRepository eventRepository, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async Task<Event> HandleAsync(CreateEventCommand command)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user == null || user.Email != "admin@admin.com")
            {
                throw new UnauthorizedAccessException("El usuario no tiene permisos de administrador.");
            }

            var newEvent = new Event
            {
                Name = command.Name,
                EventDate = command.EventDate,
                Venue = command.Venue,
                Status = "Active",
                ImageUrl = command.ImageUrl,
                Sectors = new List<Sector>()
            };

            foreach (var sectorDto in command.Sectors)
            {
                var newSector = new Sector
                {
                    Name = sectorDto.Name,
                    Price = sectorDto.Price,
                    Capacity = sectorDto.Capacity,
                    Seats = new List<Seat>()
                };

                for (int i = 1; i <= sectorDto.Capacity; i++)
                {
                    newSector.Seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        RowIdentifier = "A", // Simplificación solicitada en el proyecto
                        SeatNumber = i,
                        Status = "Available",
                        Version = 1
                    });
                }

                newEvent.Sectors.Add(newSector);
            }

            await _eventRepository.AddAsync(newEvent);
            await _unitOfWork.SaveChangesAsync();

            return newEvent;
        }
    }
}
