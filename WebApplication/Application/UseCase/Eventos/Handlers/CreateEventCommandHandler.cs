using Application.Interfaces;
using Application.UseCase.Eventos.Commands;
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public class CreateEventCommandHandler : ICreateEventCommandHandler
    {
        private readonly IEventRepository _eventRepository;

        public CreateEventCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<Event> HandleAsync(CreateEventCommand command)
        {
            var newEvent = new Event
            {
                Name = command.Name,
                EventDate = command.EventDate,
                Venue = command.Venue,
                Status = "Active", // By default active
                Sectors = new List<Sector>()
            };

            foreach (var secCmd in command.Sectors)
            {
                var sector = new Sector
                {
                    Name = secCmd.Name,
                    Price = secCmd.Price,
                    Capacity = secCmd.Capacity,
                    Seats = new List<Seat>()
                };

                // Auto-generate seats based on capacity
                for (int i = 1; i <= secCmd.Capacity; i++)
                {
                    var seat = new Seat
                    {
                        SeatNumber = i,
                        Status = "Available" // Default state
                    };
                    sector.Seats.Add(seat);
                }

                newEvent.Sectors.Add(sector);
            }

            await _eventRepository.AddAsync(newEvent);
            return newEvent;
        }
    }
}
