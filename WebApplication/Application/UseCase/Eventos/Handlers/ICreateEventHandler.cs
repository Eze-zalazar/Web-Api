using Application.DTOs;
using Application.UseCase.Eventos.Commands;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public interface ICreateEventHandler
    {
        Task<EventResponse> HandleAsync(CrearEventoCommand command);
        Task<Event> HandleAsync(CreateEventCommand command);
    }
}
