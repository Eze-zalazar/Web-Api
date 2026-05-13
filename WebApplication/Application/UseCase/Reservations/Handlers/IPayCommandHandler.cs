using Application.UseCase.Reservations.Commands;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public interface IPayCommandHandler
    {
        Task<bool> HandleAsync(PayCommand command);
    }
}
