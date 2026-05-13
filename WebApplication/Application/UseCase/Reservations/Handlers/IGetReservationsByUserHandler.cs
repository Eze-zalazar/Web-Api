using Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.UseCase.Reservations.Handlers
{
    public interface IGetReservationsByUserHandler
    {
        Task<IEnumerable<UserReservationDTO>> HandleAsync(int userId);
    }
}
