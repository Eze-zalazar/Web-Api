using Application.DTOs;
using System.Threading.Tasks;

namespace Application.UseCase.Payments.Handlers
{
    public interface IProcessPaymentHandler
    {
        Task<bool> HandleAsync(ProcessPaymentRequest request, int userId);
    }
}
