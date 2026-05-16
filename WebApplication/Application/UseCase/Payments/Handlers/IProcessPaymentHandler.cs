using Application.UseCase.Payments.Commands;
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.UseCase.Payments.Handlers
{
    public interface IProcessPaymentHandler
    {
        Task<Reservation> HandleAsync(ProcessPaymentCommand command);
    }
}
