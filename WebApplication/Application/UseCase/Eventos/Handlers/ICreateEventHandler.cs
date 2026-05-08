using Application.UseCase.Eventos.Commands;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public interface ICreateEventHandler
    {
        Task<Event> HandleAsync(CreateEventCommand command);
    }
}
