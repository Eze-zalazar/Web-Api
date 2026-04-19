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
            var evento = await _eventRepository.GetByIdAsync(query.EventId);
            if (evento == null) return null;

            return new EventResponse
            {
                Id = evento.Id,
                Name = evento.Name,
                Venue = evento.Venue,
                EventDate = evento.EventDate,
                Status = evento.Status
            };
        }
    }
}
