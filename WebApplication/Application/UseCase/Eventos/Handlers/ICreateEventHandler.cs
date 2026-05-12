using Application.DTOs;
using Application.UseCase.Eventos.Commands;
using System.Threading.Tasks;

namespace Application.UseCase.Eventos.Handlers
{
    public interface ICreateEventHandler
    {
        Task<EventResponse> HandleAsync(CrearEventoCommand command);
    }
}
