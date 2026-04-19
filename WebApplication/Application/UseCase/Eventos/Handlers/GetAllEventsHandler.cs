using Application.DTOs;
using Application.Interfaces;
using Application.UseCase.Eventos.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public class GetAllEventsHandler : IGetAllEventsHandler
    {
        private readonly IEventRepository _eventRepository;

        public GetAllEventsHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventResponse>> HandleAsync(GetAllEventsQuery query)
        {
            var events = await _eventRepository.GetAllAsync(query.Page, query.PageSize);

            return events.Select(e => new EventResponse
            {
                Id = e.Id,
                Name = e.Name,
                Venue = e.Venue,
                EventDate = e.EventDate,
                Status = e.Status
            });
        }
    }
}
