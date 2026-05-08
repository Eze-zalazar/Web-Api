using Application.UseCase.Pagos.Commands;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.UseCase.Pagos.Handlers
{
    public interface IProcessPaymentHandler
    {
        Task<Reservation> HandleAsync(ProcessPaymentCommand command);
    }
}
