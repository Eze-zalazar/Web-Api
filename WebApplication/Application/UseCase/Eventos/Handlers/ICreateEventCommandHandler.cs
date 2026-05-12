using Application.UseCase.Eventos.Commands;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public interface ICreateEventCommandHandler
    {
        Task<Event> HandleAsync(CreateEventCommand command);
    }
}
