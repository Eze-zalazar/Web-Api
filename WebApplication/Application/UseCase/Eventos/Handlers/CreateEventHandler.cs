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
        private readonly IEventRepository _eventRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEventHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
        {
            _eventRepository = eventRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<EventResponse> HandleAsync(CrearEventoCommand command)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newEvent = new Event
                {
                    Name = command.Nombre,
                    EventDate = command.FechaEvento,
                    Venue = command.Lugar,
                    Status = "Active",
                    Sectors = new List<Sector>()
                };

                foreach (var sectorReq in command.Sectores)
                {
                    var sector = new Sector
                    {
                        Name = sectorReq.Nombre,
                        Price = sectorReq.Precio,
                        Capacity = sectorReq.Capacidad,
                        Seats = new List<Seat>()
                    };

                    // Generar butacas para este sector
                    for (int i = 1; i <= sectorReq.Capacidad; i++)
                    {
                        var seat = new Seat
                        {
                            Id = Guid.NewGuid(),
                            RowIdentifier = "A", // O lógica dinámica para filas si se requiere
                            SeatNumber = i,
                            Status = "Available",
                            Version = 1
                        };
                        sector.Seats.Add(seat);
                    }

                    newEvent.Sectors.Add(sector);
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
        }
    }
}
