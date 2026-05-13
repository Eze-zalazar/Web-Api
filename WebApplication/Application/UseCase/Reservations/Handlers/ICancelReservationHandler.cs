using System;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public interface ICancelReservationHandler
    {
        Task HandleAsync(Guid reservationId);
    }
}
