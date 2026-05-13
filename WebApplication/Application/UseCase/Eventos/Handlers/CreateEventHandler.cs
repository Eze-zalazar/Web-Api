using Application.DTOs;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public CreateEventHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
        public CreateEventHandler(IUnitOfWork unitOfWork, IEventRepository eventRepository, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public async Task<EventResponse> HandleAsync(CrearEventoCommand command)
        public async Task<Event> HandleAsync(CreateEventCommand command)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user == null || user.Email != "admin@admin.com")
            {
                throw new UnauthorizedAccessException("El usuario no tiene permisos de administrador.");
            }

            var newEvent = new Event
            {
                    Name = command.Nombre,
                    EventDate = command.FechaEvento,
                    Venue = command.Lugar,
                Name = command.Name,
                EventDate = command.EventDate,
                Venue = command.Venue,
                Status = "Active",
                ImageUrl = command.ImageUrl,
                Sectors = new List<Sector>()
            };

                foreach (var sectorReq in command.Sectores)
            foreach (var sectorDto in command.Sectors)
            {
                    var sector = new Sector
                var newSector = new Sector
                {
                        Name = sectorReq.Nombre,
                        Price = sectorReq.Precio,
                        Capacity = sectorReq.Capacidad,
                    Name = sectorDto.Name,
                    Price = sectorDto.Price,
                    Capacity = sectorDto.Capacity,
                    Seats = new List<Seat>()
                };

                    // Generar butacas para este sector
                    for (int i = 1; i <= sectorReq.Capacidad; i++)
                for (int i = 1; i <= sectorDto.Capacity; i++)
                {
                        var seat = new Seat
                    newSector.Seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                            RowIdentifier = "A", // O lógica dinámica para filas si se requiere
                        RowIdentifier = "A", // Simplificación solicitada en el proyecto
                        SeatNumber = i,
                        Status = "Available",
                        Version = 1
                        };
                        sector.Seats.Add(seat);
                    });
                }

                    newEvent.Sectors.Add(sector);
                newEvent.Sectors.Add(newSector);
            }

            await _eventRepository.AddAsync(newEvent);
            await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Mapear a EventResponse
                return new EventResponse
                {
                    Id = newEvent.Id,
                    Name = newEvent.Name,
                    EventDate = newEvent.EventDate,
                    Venue = newEvent.Venue,
                    Status = newEvent.Status
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            return newEvent;
        }
    }
}
