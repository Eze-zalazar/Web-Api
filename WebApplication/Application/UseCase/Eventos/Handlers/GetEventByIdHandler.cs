using Application.DTOs;
using Application.Interfaces;
using Application.UseCase.Eventos.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public class GetEventByIdHandler : IGetEventByIdHandler
    {
        private readonly IEventRepository _eventRepository;

        public GetEventByIdHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<EventResponse?> HandleAsync(GetEventByIdQuery query)
        {
            // GetByIdWithSectorsAsync hace Include(e => e.Sectors) — ver EventRepository
            var evento = await _eventRepository.GetByIdWithSectorsAsync(query.EventId);
            if (evento == null) return null;

            return new EventResponse
            {
                Id = evento.Id,
                Name = evento.Name,
                Venue = evento.Venue,
                EventDate = evento.EventDate,
                Status = evento.Status,
                Sectors = evento.Sectors?.Select(s => new SectorInfo
                {
                    Id = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    Capacity = s.Capacity
                }) ?? Enumerable.Empty<SectorInfo>()
            };
        }
    }
}
