using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public interface IReleaseExpiredReservationsHandler
    {
        Task<int> HandleAsync();
    }
}
